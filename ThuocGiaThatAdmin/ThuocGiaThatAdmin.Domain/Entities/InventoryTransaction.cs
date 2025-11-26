using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Lịch sử giao dịch nhập/xuất kho
    /// </summary>
    public class InventoryTransaction : AuditableEntity
    {
        /// <summary>
        /// ID của ProductVariant
        /// </summary>
        public int ProductVariantId { get; set; }
        
        /// <summary>
        /// ID của Warehouse
        /// </summary>
        public int WarehouseId { get; set; }
        
        /// <summary>
        /// ID của Batch (nếu có)
        /// </summary>
        public int? BatchId { get; set; }
        
        /// <summary>
        /// Loại giao dịch
        /// </summary>
        public TransactionType Type { get; set; }
        
        /// <summary>
        /// Số lượng (+ nhập, - xuất)
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// Số lượng trước giao dịch
        /// </summary>
        public int QuantityBefore { get; set; }
        
        /// <summary>
        /// Số lượng sau giao dịch
        /// </summary>
        public int QuantityAfter { get; set; }
        
        /// <summary>
        /// Giá trị đơn giá
        /// </summary>
        public decimal? UnitPrice { get; set; }
        
        /// <summary>
        /// Tổng giá trị
        /// </summary>
        public decimal? TotalValue { get; set; }
        
        /// <summary>
        /// Số tham chiếu (Order ID, Purchase Order, etc.)
        /// </summary>
        public string? ReferenceNumber { get; set; }
        
        /// <summary>
        /// Loại tham chiếu
        /// </summary>
        public string? ReferenceType { get; set; }
        
        /// <summary>
        /// ID người thực hiện
        /// </summary>
        public string? PerformedByUserId { get; set; }
        
        /// <summary>
        /// Lý do
        /// </summary>
        public string? Reason { get; set; }
        
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Notes { get; set; }
        
        // Navigation properties
        public ProductVariant ProductVariant { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
        public InventoryBatch? Batch { get; set; }
    }
}
