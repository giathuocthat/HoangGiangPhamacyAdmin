using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository interface for GoodsReceipt entity
    /// </summary>
    public interface IGoodsReceiptRepository : IRepository<GoodsReceipt>
    {
        /// <summary>
        /// Get goods receipt by receipt number
        /// </summary>
        Task<GoodsReceipt?> GetByReceiptNumberAsync(string receiptNumber);

        /// <summary>
        /// Get goods receipt with full details (items)
        /// </summary>
        Task<GoodsReceipt?> GetWithDetailsAsync(int id);

        /// <summary>
        /// Get goods receipts by purchase order ID
        /// </summary>
        Task<IEnumerable<GoodsReceipt>> GetByPurchaseOrderIdAsync(int purchaseOrderId);

        /// <summary>
        /// Get goods receipts by warehouse ID
        /// </summary>
        Task<IEnumerable<GoodsReceipt>> GetByWarehouseIdAsync(int warehouseId);

        /// <summary>
        /// Get goods receipts by status
        /// </summary>
        Task<IEnumerable<GoodsReceipt>> GetByStatusAsync(GoodsReceiptStatus status);

        /// <summary>
        /// Get goods receipts with paging and filtering
        /// </summary>
        Task<(IList<GoodsReceipt> receipts, int totalCount)> GetPagedGoodsReceiptsAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            int? purchaseOrderId = null,
            int? warehouseId = null,
            GoodsReceiptStatus? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        /// <summary>
        /// Generate next receipt number
        /// </summary>
        Task<string> GenerateReceiptNumberAsync();

        /// <summary>
        /// Check if receipt number exists
        /// </summary>
        Task<bool> ReceiptNumberExistsAsync(string receiptNumber);

        Task<(IList<GoodsReceipt> receipts, int totalCount)> GetFilteredGoodsReceiptsAsync(FilterRequest request);
    }

    /// <summary>
    /// Repository interface for GoodsReceiptItem entity
    /// </summary>
    public interface IGoodsReceiptItemRepository : IRepository<GoodsReceiptItem>
    {
        /// <summary>
        /// Get items by goods receipt ID
        /// </summary>
        Task<IEnumerable<GoodsReceiptItem>> GetByGoodsReceiptIdAsync(int goodsReceiptId);

        /// <summary>
        /// Get items by purchase order item ID
        /// </summary>
        Task<IEnumerable<GoodsReceiptItem>> GetByPurchaseOrderItemIdAsync(int purchaseOrderItemId);

        /// <summary>
        /// Get item with purchase order item details
        /// </summary>
        Task<GoodsReceiptItem?> GetWithPurchaseOrderItemAsync(int id);
    }
}
