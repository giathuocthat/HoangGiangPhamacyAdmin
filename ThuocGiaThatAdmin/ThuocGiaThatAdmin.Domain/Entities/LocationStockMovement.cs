using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Lịch sử di chuyển variant giữa các vị trí trong kho
    /// </summary>
    public class LocationStockMovement : AuditableEntity
    {
        public int ProductVariantId { get; set; }
        public int WarehouseId { get; set; }
        
        /// <summary>
        /// Mã vị trí nguồn (có thể null nếu nhập kho mới)
        /// Ví dụ: "KA-R1-S2-B3"
        /// </summary>
        public string? FromLocationCode { get; set; }
        
        /// <summary>
        /// Mã vị trí đích (có thể null nếu xuất kho)
        /// Ví dụ: "KB-R2-S1-B5"
        /// </summary>
        public string? ToLocationCode { get; set; }
        
        /// <summary>
        /// Số lô hàng (batch number) được di chuyển
        /// Ví dụ: "LOT-2024-001"
        /// </summary>
        public string BatchNumber { get; set; } = string.Empty;
        
        public int Quantity { get; set; }
        
        public string? Reason { get; set; } // Lý do di chuyển
        
        public int? MovedByUserId { get; set; } // User thực hiện
        
        public DateTime MovementDate { get; set; }
        
        // Navigation properties
        public ProductVariant ProductVariant { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
    }
}
