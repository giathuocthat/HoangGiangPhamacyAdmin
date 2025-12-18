using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    /// <summary>
    /// Service interface for CustomerPaymentAccount business logic
    /// </summary>
    public interface ICustomerPaymentAccountService
    {
        Task<IEnumerable<CustomerPaymentAccountDto>> GetAllAsync();
        Task<CustomerPaymentAccountDto?> GetByIdAsync(int id);
        Task<IEnumerable<CustomerPaymentAccountDto>> GetByCustomerIdAsync(int customerId);
        Task<CustomerPaymentAccountDto?> GetDefaultAccountAsync(int customerId);
        Task<CustomerPaymentAccountDto> CreateAsync(CreateCustomerPaymentAccountDto dto);
        Task<CustomerPaymentAccountDto> UpdateAsync(int id, UpdateCustomerPaymentAccountDto dto);
        Task DeleteAsync(int id);
        Task SetDefaultAccountAsync(int accountId, int customerId);
    }
}
