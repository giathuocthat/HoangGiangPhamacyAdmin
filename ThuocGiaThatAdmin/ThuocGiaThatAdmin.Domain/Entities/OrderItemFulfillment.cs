using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// OrderItemFulfillment - Theo dõi việc book hàng từ các lô cụ thể cho từng OrderItem
    /// Mỗi record đại diện cho việc allocate một số lượng từ một InventoryBatch cho một OrderItem
    /// </summary>
    public class OrderItemFulfillment : AuditableEntity
    {
        /// <summary>
        /// ID của OrderItem được fulfill
        /// </summary>
        public int OrderItemId { get; set; }
        
        /// <summary>
        /// ID của lô hàng được book
        /// </summary>
        public int InventoryBatchId { get; set; }
        
        /// <summary>
        /// Số lượng được book từ lô này
        /// </summary>
        public int QuantityFulfilled { get; set; }
        
        /// <summary>
        /// Thời gian thực hiện fulfill
        /// </summary>
        public DateTime FulfilledDate { get; set; }
        
        /// <summary>
        /// User thực hiện fulfill (Admin/Warehouse Manager)
        /// </summary>
        public Guid FulfilledByUserId { get; set; }
        
        /// <summary>
        /// Ghi chú (optional)
        /// </summary>
        public string? Notes { get; set; }
        
        // Navigation properties
        public OrderItem OrderItem { get; set; } = null!;
        public InventoryBatch InventoryBatch { get; set; } = null!;
    }
}
