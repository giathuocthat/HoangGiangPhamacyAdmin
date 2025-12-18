using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    /// <summary>
    /// Service interface for Bank operations
    /// </summary>
    public interface IBankService
    {
        Task<List<BankDto>> GetAllBanksAsync();
        Task<List<BankDto>> GetActiveBanksAsync();
        Task<BankDto?> GetByIdAsync(int id);
        Task<BankDto?> GetByCodeAsync(string bankCode);
        Task<BankDto> CreateAsync(CreateBankDto dto);
        Task<BankDto> UpdateAsync(int id, UpdateBankDto dto);
        Task DeleteAsync(int id);
    }
}
