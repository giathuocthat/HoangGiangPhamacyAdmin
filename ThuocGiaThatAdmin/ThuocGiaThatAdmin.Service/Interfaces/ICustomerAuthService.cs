using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface ICustomerAuthService
    {
        Task<(bool Success, string Message, CustomerProfileTokenDto Customer)> RegisterAsync(CustomerRegisterDto dto);
        Task<(bool Success, string Message, string? Token, string? expiresAt, Customer? Customer)> LoginAsync(CustomerLoginDto dto);
        Task<Customer?> GetCustomerByIdAsync(int customerId);
        Task<Customer?> GetCustomerByEmailAsync(string email);
        Task<bool> UpdateProfileAsync(int customerId, UpdateCustomerProfileDto dto);
        Task<bool> UpdateBusinessInfoAsync(int customerId, UpdateBusinessInfoDto dto);
        string GenerateJwtToken(Customer customer);
        (string token, string expiresAt) GenerateJwtTokenAndExpires(Customer customer);
        bool VerifyPassword(string password, string passwordHash);
        string HashPassword(string password);
        Task<(bool success, Dictionary<string, string>)> VerifyRegister(string phone, string email);
    }
}
