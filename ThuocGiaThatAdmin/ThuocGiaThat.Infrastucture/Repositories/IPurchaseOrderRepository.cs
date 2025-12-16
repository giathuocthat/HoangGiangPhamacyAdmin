using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository interface for PurchaseOrder entity
    /// </summary>
    public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
    {
        /// <summary>
        /// Get purchase order by order number
        /// </summary>
        Task<PurchaseOrder?> GetByOrderNumberAsync(string orderNumber);

        /// <summary>
        /// Get purchase order with full details (items, receipts, history)
        /// </summary>
        Task<PurchaseOrder?> GetWithDetailsAsync(int id);

        /// <summary>
        /// Get purchase orders by supplier ID
        /// </summary>
        Task<IEnumerable<PurchaseOrder>> GetBySupplierIdAsync(int supplierId);

        /// <summary>
        /// Get purchase orders by warehouse ID
        /// </summary>
        Task<IEnumerable<PurchaseOrder>> GetByWarehouseIdAsync(int warehouseId);

        /// <summary>
        /// Get purchase orders by status
        /// </summary>
        Task<IEnumerable<PurchaseOrder>> GetByStatusAsync(PurchaseOrderStatus status);

        /// <summary>
        /// Get purchase orders with paging and filtering
        /// </summary>
        Task<(IList<PurchaseOrder> orders, int totalCount)> GetPagedPurchaseOrdersAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            int? supplierId = null,
            int? warehouseId = null,
            PurchaseOrderStatus? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        /// <summary>
        /// Generate next order number
        /// </summary>
        Task<string> GenerateOrderNumberAsync();

        /// <summary>
        /// Check if order number exists
        /// </summary>
        Task<bool> OrderNumberExistsAsync(string orderNumber);
    }

    /// <summary>
    /// Repository interface for PurchaseOrderItem entity
    /// </summary>
    public interface IPurchaseOrderItemRepository : IRepository<PurchaseOrderItem>
    {
        /// <summary>
        /// Get items by purchase order ID
        /// </summary>
        Task<IEnumerable<PurchaseOrderItem>> GetByPurchaseOrderIdAsync(int purchaseOrderId);

        /// <summary>
        /// Get item with product variant details
        /// </summary>
        Task<PurchaseOrderItem?> GetWithProductVariantAsync(int id);
    }

    /// <summary>
    /// Repository interface for PurchaseOrderHistory entity
    /// </summary>
    public interface IPurchaseOrderHistoryRepository : IRepository<PurchaseOrderHistory>
    {
        /// <summary>
        /// Get history by purchase order ID
        /// </summary>
        Task<IEnumerable<PurchaseOrderHistory>> GetByPurchaseOrderIdAsync(int purchaseOrderId);

        /// <summary>
        /// Get latest history entry for purchase order
        /// </summary>
        Task<PurchaseOrderHistory?> GetLatestByPurchaseOrderIdAsync(int purchaseOrderId);
    }
}
