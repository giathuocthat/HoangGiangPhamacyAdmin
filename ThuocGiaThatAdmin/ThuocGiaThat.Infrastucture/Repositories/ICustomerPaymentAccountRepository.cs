using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository interface for CustomerPaymentAccount operations
    /// </summary>
    public interface ICustomerPaymentAccountRepository
    {
        Task<IEnumerable<CustomerPaymentAccountDto>> GetAllAsync();
        Task<CustomerPaymentAccountDto?> GetByIdAsync(int id);
        Task<IEnumerable<CustomerPaymentAccountDto>> GetByCustomerIdAsync(int customerId);
        Task<CustomerPaymentAccountDto?> GetDefaultAccountAsync(int customerId);
        Task<CustomerPaymentAccountDto> CreateAsync(CustomerPaymentAccountDto dto);
        Task<CustomerPaymentAccountDto> UpdateAsync(CustomerPaymentAccountDto dto);
        Task DeleteAsync(int id);
        Task SetDefaultAccountAsync(int accountId, int customerId);
        Task<bool> AccountExistsAsync(int id);
    }
}
