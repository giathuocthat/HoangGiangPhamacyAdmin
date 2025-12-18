using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository interface for Bank entity
    /// </summary>
    public interface IBankRepository : IRepository<Bank>
    {
        Task<List<BankDto>> GetAllBanksAsync();
        Task<List<BankDto>> GetActiveBanksAsync();
        Task<BankDto?> GetBankByIdAsync(int id);
        Task<BankDto?> GetBankByCodeAsync(string bankCode);
        Task<Bank> CreateAsync(Bank bank);
        Task<Bank> UpdateAsync(Bank bank);
        Task DeleteAsync(int id);
        Task<bool> BankCodeExistsAsync(string bankCode, int? excludeId = null);
    }
}
