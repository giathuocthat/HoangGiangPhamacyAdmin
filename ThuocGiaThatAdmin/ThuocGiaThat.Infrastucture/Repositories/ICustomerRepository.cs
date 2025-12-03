using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        /// <summary>
        /// Get customer by ID with related entities (BusinessType, Addresses)
        /// </summary>
        Task<Customer?> GetByIdWithIncludesAsync(int id);

        /// <summary>
        /// Get all customers with pagination and related entities
        /// </summary>
        Task<(IEnumerable<Customer> Customers, int TotalCount)> GetAllWithPaginationAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Check if phone number exists (case-insensitive)
        /// </summary>
        Task<bool> IsPhoneNumberExistsAsync(string phoneNumber, int? excludeCustomerId = null);

        /// <summary>
        /// Check if email exists (case-insensitive)
        /// </summary>
        Task<bool> IsEmailExistsAsync(string email, int? excludeCustomerId = null);

        /// <summary>
        /// Search customers by phone number (partial match)
        /// </summary>
        Task<IEnumerable<Customer>> SearchByPhoneAsync(string phoneNumber);
    }
}
