using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    /// <summary>
    /// Service interface for Supplier business logic
    /// </summary>
    public interface ISupplierService
    {
        // CRUD Operations
        Task<IEnumerable<SupplierDto>> GetAllAsync();
        Task<SupplierDto?> GetByIdAsync(int id);
        Task<SupplierDto?> GetByCodeAsync(string code);
        Task<IEnumerable<SupplierDto>> GetActiveSuppliersAsync();
        Task<SupplierDto> CreateAsync(CreateSupplierDto dto);
        Task<SupplierDto> UpdateAsync(int id, UpdateSupplierDto dto);
        Task DeleteAsync(int id);

        // Query
        Task<SupplierDto?> GetWithContactsAsync(int id);
        Task<(IEnumerable<SupplierListItemDto>, int totalCount)> GetPagedSuppliersAsync(
            int pageNumber = 1,
            int pageSize = 20,
            string? searchTerm = null,
            bool? isActive = null);
    }

    /// <summary>
    /// Service interface for SupplierContact business logic
    /// </summary>
    public interface ISupplierContactService
    {
        Task<IEnumerable<SupplierContactDto>> GetBySupplierIdAsync(int supplierId);
        Task<SupplierContactDto?> GetByIdAsync(int id);
        Task<SupplierContactDto> CreateAsync(CreateSupplierContactDto dto);
        Task<SupplierContactDto> UpdateAsync(int id, UpdateSupplierContactDto dto);
        Task DeleteAsync(int id);
    }
}
