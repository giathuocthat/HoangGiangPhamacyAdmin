using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Cảnh báo tồn kho
    /// </summary>
    public class StockAlert : AuditableEntity
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
        /// ID của Batch (nếu cảnh báo hết hạn)
        /// </summary>
        public int? BatchId { get; set; }
        
        /// <summary>
        /// Loại cảnh báo
        /// </summary>
        public AlertType Type { get; set; }
        
        /// <summary>
        /// Mức độ ưu tiên
        /// </summary>
        public AlertPriority Priority { get; set; }
        
        /// <summary>
        /// Thông báo
        /// </summary>
        public string Message { get; set; } = string.Empty;
        
        /// <summary>
        /// Số lượng hiện tại
        /// </summary>
        public int CurrentQuantity { get; set; }
        
        /// <summary>
        /// Ngày hết hạn (nếu là cảnh báo hết hạn)
        /// </summary>
        public DateTime? ExpiryDate { get; set; }
        
        /// <summary>
        /// Đã đọc chưa
        /// </summary>
        public bool IsRead { get; set; } = false;
        
        /// <summary>
        /// Đã xử lý chưa
        /// </summary>
        public bool IsResolved { get; set; } = false;
        
        /// <summary>
        /// Ngày xử lý
        /// </summary>
        public DateTime? ResolvedDate { get; set; }
        
        /// <summary>
        /// Người xử lý
        /// </summary>
        public string? ResolvedByUserId { get; set; }
        
        /// <summary>
        /// Ghi chú xử lý
        /// </summary>
        public string? ResolutionNotes { get; set; }
        
        // Navigation properties
        public ProductVariant ProductVariant { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
        public InventoryBatch? Batch { get; set; }
    }
}
