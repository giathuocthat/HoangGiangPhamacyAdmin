using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface ICustomerService
    {
        /// <summary>
        /// Create a new customer with validation
        /// </summary>
        Task<(bool Success, string Message, CustomerResponseDto? Customer)> CreateCustomerAsync(CreateCustomerDto dto);

        /// <summary>
        /// Update existing customer with validation
        /// </summary>
        Task<(bool Success, string Message, CustomerResponseDto? Customer)> UpdateCustomerAsync(int id, UpdateCustomerDto dto);

        /// <summary>
        /// Get customer by ID
        /// </summary>
        Task<CustomerResponseDto?> GetCustomerByIdAsync(int id);

        /// <summary>
        /// Get all customers with pagination
        /// </summary>
        Task<(IEnumerable<CustomerResponseDto> Customers, int TotalCount)> GetCustomersAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Search customers by phone number
        /// </summary>
        Task<IEnumerable<CustomerResponseDto>> SearchByPhoneAsync(string phoneNumber);
    }
}
