using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.Responses;
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
        Task<IEnumerable<PurchaseOrderResponse>> GetAllAsync();
        Task<PurchaseOrderResponse?> GetByIdAsync(int id);
        Task<PurchaseOrderResponse?> GetByOrderNumberAsync(string orderNumber);
        Task<PurchaseOrderResponse> CreateAsync(CreatePurchaseOrderDto dto, int createdByUserId);
        Task<PurchaseOrderResponse> UpdateAsync(int id, UpdatePurchaseOrderDto dto);
        Task DeleteAsync(int id);

        // Query
        Task<PurchaseOrderResponse?> GetWithDetailsAsync(int id);
        Task<IEnumerable<PurchaseOrderResponse>> GetBySupplierIdAsync(int supplierId);
        Task<IEnumerable<PurchaseOrderResponse>> GetByWarehouseIdAsync(int warehouseId);
        Task<IEnumerable<PurchaseOrderResponse>> GetByStatusAsync(PurchaseOrderStatus status);
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
        Task<PurchaseOrderResponse> ApproveAsync(int id, int approvedByUserId, ApprovePurchaseOrderDto dto);
        Task<PurchaseOrderResponse> CancelAsync(int id, int cancelledByUserId, CancelPurchaseOrderDto dto);
        Task<string> GenerateOrderNumberAsync();
    }
}
