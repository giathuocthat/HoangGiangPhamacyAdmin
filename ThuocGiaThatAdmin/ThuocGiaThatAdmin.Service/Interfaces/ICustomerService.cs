using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contract.Requests;
using ThuocGiaThatAdmin.Contract.Responses;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documents"></param>
        /// <returns></returns>
        Task UpdateLicenses(IList<CustomerDocumentDto> documents);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        Task<IList<CustomerLicenseResponse>> GetLicenses(int customerId);
    }
}
