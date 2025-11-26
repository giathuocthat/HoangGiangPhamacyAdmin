using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Lô hàng - Theo dõi hạn sử dụng
    /// </summary>
    public class InventoryBatch : AuditableEntity
    {
        /// <summary>
        /// ID của Inventory
        /// </summary>
        public int InventoryId { get; set; }
        
        /// <summary>
        /// Số lô (batch number)
        /// </summary>
        public string BatchNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// Ngày sản xuất
        /// </summary>
        public DateTime? ManufactureDate { get; set; }
        
        /// <summary>
        /// Hạn sử dụng (QUAN TRỌNG!)
        /// </summary>
        public DateTime ExpiryDate { get; set; }
        
        /// <summary>
        /// Số lượng trong lô này
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// Số lượng đã bán
        /// </summary>
        public int QuantitySold { get; set; } = 0;
        
        /// <summary>
        /// Số lượng còn lại
        /// </summary>
        public int QuantityRemaining => Quantity - QuantitySold;
        
        /// <summary>
        /// Giá nhập của lô này
        /// </summary>
        public decimal CostPrice { get; set; }
        
        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public string? Supplier { get; set; }
        
        /// <summary>
        /// Số hóa đơn nhập hàng
        /// </summary>
        public string? PurchaseOrderNumber { get; set; }
        
        /// <summary>
        /// Trạng thái lô hàng
        /// </summary>
        public BatchStatus Status { get; set; } = BatchStatus.Active;
        
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Notes { get; set; }
        
        // Navigation properties
        public Inventory Inventory { get; set; } = null!;
    }
}
