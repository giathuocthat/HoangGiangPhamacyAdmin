using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    /// <summary>
    /// Service interface for PurchaseOrder business logic
    /// </summary>
    public interface IPurchaseOrderService
    {
        // CRUD Operations
        Task<IEnumerable<PurchaseOrderDto>> GetAllAsync();
        Task<PurchaseOrderDto?> GetByIdAsync(int id);
        Task<PurchaseOrderDto?> GetByOrderNumberAsync(string orderNumber);
        Task<PurchaseOrderDto> CreateAsync(CreatePurchaseOrderDto dto, int createdByUserId);
        Task<PurchaseOrderDto> UpdateAsync(int id, UpdatePurchaseOrderDto dto);
        Task DeleteAsync(int id);

        // Query
        Task<PurchaseOrderDto?> GetWithDetailsAsync(int id);
        Task<IEnumerable<PurchaseOrderDto>> GetBySupplierIdAsync(int supplierId);
        Task<IEnumerable<PurchaseOrderDto>> GetByWarehouseIdAsync(int warehouseId);
        Task<IEnumerable<PurchaseOrderDto>> GetByStatusAsync(PurchaseOrderStatus status);
        Task<(IEnumerable<PurchaseOrderListItemDto>, int totalCount)> GetPagedPurchaseOrdersAsync(
            int pageNumber = 1,
            int pageSize = 20,
            string? searchTerm = null,
            int? supplierId = null,
            int? warehouseId = null,
            PurchaseOrderStatus? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        // Business Operations
        Task<PurchaseOrderDto> ApproveAsync(int id, int approvedByUserId, ApprovePurchaseOrderDto dto);
        Task<PurchaseOrderDto> CancelAsync(int id, int cancelledByUserId, CancelPurchaseOrderDto dto);
        Task<string> GenerateOrderNumberAsync();
    }
}
