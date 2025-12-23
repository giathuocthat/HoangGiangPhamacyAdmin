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
using ThuocGiaThatAdmin.Contract.Enums;
using ThuocGiaThatAdmin.Contract.Requests;
using ThuocGiaThatAdmin.Contract.Responses;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class CustomerAuthService : ICustomerAuthService
    {
        private readonly TrueMecContext _context;
        private readonly IConfiguration _configuration;
        private readonly IZaloService _zaloService;

        public CustomerAuthService(TrueMecContext context, IConfiguration configuration, IZaloService zaloService)
        {
            _context = context;
            _configuration = configuration;
            _zaloService = zaloService;
        }

        public async Task<(bool Success, string Message, CustomerProfileTokenDto? Customer)> RegisterAsync(CustomerRegisterDto dto)
        {
            // Check if phoneNumber or email already exists
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.PhoneNumber == dto.PhoneNumber || c.Email == dto.Email);

            if (existingCustomer != null)
            {
                return existingCustomer.PhoneNumber == dto.PhoneNumber ? (false, "Phone Number has been already registered", null) : (false, "Email has been already registered", null);
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
                CreatedDate = DateTime.UtcNow,
                Email = dto.Email
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
                Id = customer.Id,
                IsVerified = customer.IsVerified
            };


            return (true, "Registration successful", result);
        }

        public async Task<(bool Success, string? accessToken, string? refreshToken, Customer? Customer)> LoginAsync(CustomerLoginDto dto)
        {
            var customer = await _context.Customers
                .Include(c => c.BusinessType)
                .FirstOrDefaultAsync(c => c.PhoneNumber == dto.PhoneNumber);

            if (customer == null)
            {
                return (false, null, null, null);
            }

            if (!VerifyPassword(dto.Password, customer.PasswordHash))
            {
                return (false, null, null, null);
            }

            var jwtSettings = _configuration.GetSection("Jwt");

            var accessTokenExpires = DateTime.Now.AddMinutes(int.Parse(jwtSettings["ExpiresMinutes"] ?? "60"));
            var accessToken = GenerateJwtToken(customer, accessTokenExpires);

            var refreshTokenExpires = dto.RememberMe == true ? DateTime.Now.AddDays(7) : DateTime.Now.AddDays(1);
            var refreshToken = GenerateJwtToken(customer, refreshTokenExpires);

            return (true, accessToken, refreshToken, customer);
        }

        private string GenerateJwtToken(Customer customer, DateTime expires)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured");
            var issuer = jwtSettings["Issuer"] ?? "ThuocGiaThatAPI";
            var audience = jwtSettings["Audience"] ?? "ThuocGiaThatCustomers";
            var expiryMinutes = int.Parse(jwtSettings["ExpiresMinutes"] ?? "60");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, customer.FullName),
                new Claim(ClaimTypes.Role, "Customer"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string Refresh(string refreshToken)
        {
            var principal = ValidateRefreshToken(refreshToken);
            var customerId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var fullName = principal.FindFirstValue(JwtRegisteredClaimNames.Name);
            string accessToken = GenerateJwtToken(new Customer { Id = int.Parse(customerId), FullName = fullName });

            return accessToken;
        } 

        public ClaimsPrincipal ValidateRefreshToken(string refreshToken)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var issuer = jwtSettings["Issuer"] ?? "ThuocGiaThatAPI";
            var audience = jwtSettings["Audience"] ?? "ThuocGiaThatCustomers";

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            try
            {
                var principal = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    ValidateAudience = true,
                    ValidAudience = audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    RequireExpirationTime = true
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                throw new SecurityTokenException("Refresh token expired");
            }
            catch (Exception)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }
        }

        public async Task<(bool Success, string Message, string? Token, string? expiresAt, Customer? Customer)> LoginByOtpAsync(string phoneNumber, OtpCodeTypeEnum type, string otp)
        {
            var customer = await _context.Customers
                .Include(c => c.BusinessType)
                .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);

            if (customer == null)
            {
                return (false, "Phone number does not exist", null, null, null);
            }

            var isSuccess = await _zaloService.VerifyOtpAsync(phoneNumber, type, otp);
            if (!isSuccess) return (false, "Otp is not valid", null, null, null); ;

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
            customer.Email = dto.Email;
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

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, customer.FullName),                
                new Claim(ClaimTypes.Role, "Customer"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
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
            var isExistingEmail = await _context.Customers.AnyAsync(x => x.Email == email);

            var dictionnary = new Dictionary<string, string>();
            if (isExistingPhone) dictionnary.Add("phoneNumber", "Số điện thoại đã tồn tại");
            if (isExistingEmail) dictionnary.Add("email", "Email đã tồn tại");

            if (dictionnary.Keys.Any()) return (false, dictionnary);

            return (true, dictionnary);
        }

        public async Task<(bool success, string message)> ChangePasswordAsync(int customerId, UpdateCustomerPasswordDto dto)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == customerId);
            if (customer == null) return (false, "User không tồn tại, vui lòng reload và thử lại.");

            if (!VerifyPassword(dto.OldPassword, customer.PasswordHash)) return (false, "Mật khẩu cũ không đúng");

            customer.PasswordHash = HashPassword(dto.NewPassword);

            await _context.SaveChangesAsync();

            return (true, "");
        }

        public async Task<ValidationResponse> CheckExisting(CheckCustomerExistsRequest request)
        {
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.PhoneNumber == request.PhoneNumber || c.Email == request.Email);

            var result = new ValidationResponse();

            if (existingCustomer == null) return result;

            result.IsValid = false;
            Dictionary<string, string> dict = new Dictionary<string, string>();

            if (existingCustomer.PhoneNumber == request.PhoneNumber) result.Errors.Add(new ValidationError
            {
                FieldName = "phoneNumber",
                ErrorCode = "phoneNumber_EXISTS",
                ErrorMessage = "Số điện thoại đã tồn tại"
            });

            if (existingCustomer.Email == request.Email) result.Errors.Add(new ValidationError
            {
                FieldName = "email",
                ErrorCode = "email_EXISTS",
                ErrorMessage = "Email đã tồn tại"
            });

            return result;
        }
    }
}
