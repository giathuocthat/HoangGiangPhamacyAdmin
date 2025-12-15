using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository interface for Supplier entity
    /// </summary>
    public interface ISupplierRepository : IRepository<Supplier>
    {
        /// <summary>
        /// Get supplier by code
        /// </summary>
        Task<Supplier?> GetByCodeAsync(string code);

        /// <summary>
        /// Get supplier with contacts
        /// </summary>
        Task<Supplier?> GetWithContactsAsync(int id);

        /// <summary>
        /// Get all active suppliers
        /// </summary>
        Task<IEnumerable<Supplier>> GetActiveSuppliersAsync();

        /// <summary>
        /// Get suppliers with paging
        /// </summary>
        Task<(IList<Supplier> suppliers, int totalCount)> GetPagedSuppliersAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            bool? isActive = null);

        /// <summary>
        /// Check if supplier code exists
        /// </summary>
        Task<bool> CodeExistsAsync(string code, int? excludeId = null);
    }

    /// <summary>
    /// Repository interface for SupplierContact entity
    /// </summary>
    public interface ISupplierContactRepository : IRepository<SupplierContact>
    {
        /// <summary>
        /// Get contacts by supplier ID
        /// </summary>
        Task<IEnumerable<SupplierContact>> GetBySupplierIdAsync(int supplierId);

        /// <summary>
        /// Get primary contact for supplier
        /// </summary>
        Task<SupplierContact?> GetPrimaryContactAsync(int supplierId);

        /// <summary>
        /// Get active contacts by supplier ID
        /// </summary>
        Task<IEnumerable<SupplierContact>> GetActiveContactsBySupplierIdAsync(int supplierId);
    }
}
