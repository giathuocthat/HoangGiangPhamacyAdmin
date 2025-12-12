using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class CustomerAuthService : ICustomerAuthService
    {
        private readonly TrueMecContext _context;
        private readonly IConfiguration _configuration;

        public CustomerAuthService(TrueMecContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<(bool Success, string Message, CustomerProfileTokenDto Customer)> RegisterAsync(CustomerRegisterDto dto)
        {
            // Check if email already exists
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.PhoneNumber == dto.PhoneNumber);

            if (existingCustomer != null)
            {
                return (false, "Phone Number has been already registered", null);
            }

            var businessType = await _context.BusinessTypes
                .FirstOrDefaultAsync(bt => bt.Id == dto.BusinessTypeId);

            // Create new customer
            var customer = new Customer
            {
                FullName = businessType?.Name,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = HashPassword(dto.Password),
                BusinessTypeId = dto.BusinessTypeId,
                CreatedDate = DateTime.UtcNow
            };

            customer.Addresses = new System.Collections.Generic.List<Address>
            {
                new Address
                {
                    AddressLine = dto.Address,
                    IsDefault = true,
                    CreatedDate = DateTime.UtcNow,
                    ProvinceId = dto.ProvinceId,
                    WardId = dto.WardId,
                    RecipientName = customer?.FullName ?? "",
                    PhoneNumber = dto.PhoneNumber
                }
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var (token, expiresAt) = GenerateJwtTokenAndExpires(customer);

            var result = new CustomerProfileTokenDto
            { 
                FullName = customer.FullName,
                BusinessTypeId = customer.BusinessTypeId,
                BusinessTypeName = businessType?.Name,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                ExpiresAt = expiresAt,
                Token = token,
                Id = customer.Id
            };


            return (true, "Registration successful", result);
        }

        public async Task<(bool Success, string Message, string? Token, string? expiresAt, Customer? Customer)> LoginAsync(CustomerLoginDto dto)
        {
            var customer = await _context.Customers
                .Include(c => c.BusinessType)
                .FirstOrDefaultAsync(c => c.PhoneNumber == dto.PhoneNumber);

            if (customer == null)
            {
                return (false, "Invalid phone number or password", null, null, null);
            }

            if (!VerifyPassword(dto.Password, customer.PasswordHash))
            {
                return (false, "Invalid email or password", null, null, null);
            }

            var (token, expiresAt) = GenerateJwtTokenAndExpires(customer);
            return (true, "Login successful", token, expiresAt, customer);
        }

        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            return await _context.Customers
                .Include(c => c.BusinessType)
                .Include(c => c.PaymentAccounts)
                .FirstOrDefaultAsync(c => c.Id == customerId);
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<bool> UpdateProfileAsync(int customerId, UpdateCustomerProfileDto dto)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null) return false;

            customer.FullName = dto.FullName;
            customer.PhoneNumber = dto.PhoneNumber;
            customer.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateBusinessInfoAsync(int customerId, UpdateBusinessInfoDto dto)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null) return false;

            // Verify BusinessType exists
            var businessTypeExists = await _context.BusinessTypes
                .AnyAsync(bt => bt.Id == dto.BusinessTypeId);
            if (!businessTypeExists) return false;

            customer.BusinessTypeId = dto.BusinessTypeId;
            customer.CompanyName = dto.CompanyName;
            customer.TaxCode = dto.TaxCode;
            customer.BusinessRegistrationNumber = dto.BusinessRegistrationNumber;
            customer.BusinessRegistrationDate = dto.BusinessRegistrationDate;
            customer.LegalRepresentative = dto.LegalRepresentative;
            customer.BusinessLicenseUrl = dto.BusinessLicenseUrl;
            customer.BusinessAddress = dto.BusinessAddress;
            customer.BusinessPhone = dto.BusinessPhone;
            customer.BusinessEmail = dto.BusinessEmail;
            customer.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public string GenerateJwtToken(Customer customer)
        {
            JwtSecurityToken token = BuildJwtToken(customer);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private JwtSecurityToken BuildJwtToken(Customer customer)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured");
            var issuer = jwtSettings["Issuer"] ?? "ThuocGiaThatAPI";
            var audience = jwtSettings["Audience"] ?? "ThuocGiaThatCustomers";
            var expiryMinutes = int.Parse(jwtSettings["ExpiresMinutes"] ?? "60");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),                
                new Claim(JwtRegisteredClaimNames.Name, customer.FullName),
                new Claim("customer_id", customer.Id.ToString()),
                new Claim(ClaimTypes.Role, "Customer"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );
            return token;
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        public (string token, string expiresAt) GenerateJwtTokenAndExpires(Customer customer)
        {
            JwtSecurityToken token = BuildJwtToken(customer);

            return (new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo.ToString("o"));
        }

        public async Task<(bool success, Dictionary<string, string>)> VerifyRegister(string phone, string email)
        {
            var isExistingPhone = await _context.Customers.AnyAsync(x => x.PhoneNumber == phone);
            var isExistingEmail = await _context.Customers.AnyAsync(x => x.Email ==  email);

            var dictionnary = new Dictionary<string, string>();
            if (isExistingPhone) dictionnary.Add("phoneNumber", "Số điện thoại đã tồn tại");
            if (isExistingEmail) dictionnary.Add("email", "Email đã tồn tại");

            if (dictionnary.Keys.Any()) return (false, dictionnary);

            return (true, dictionnary);
        }
    }
}
