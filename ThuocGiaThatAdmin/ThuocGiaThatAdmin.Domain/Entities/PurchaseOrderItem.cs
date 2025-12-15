using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Chi tiết sản phẩm trong đơn đặt hàng
    /// </summary>
    public class PurchaseOrderItem : AuditableEntity
    {
        // Id inherited from AuditableEntity

        /// <summary>
        /// Đơn đặt hàng
        /// </summary>
        public int PurchaseOrderId { get; set; }

        /// <summary>
        /// Sản phẩm variant
        /// </summary>
        public int ProductVariantId { get; set; }

        /// <summary>
        /// Số lượng đặt hàng
        /// </summary>
        public int OrderedQuantity { get; set; }

        /// <summary>
        /// Số lượng đã nhận (tổng từ các lần nhập)
        /// </summary>
        public int ReceivedQuantity { get; set; } = 0;

        /// <summary>
        /// Đơn giá
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Thuế suất (%)
        /// </summary>
        public decimal TaxRate { get; set; } = 0;

        /// <summary>
        /// Tiền giảm giá
        /// </summary>
        public decimal DiscountAmount { get; set; } = 0;

        /// <summary>
        /// Tổng tiền
        /// </summary>
        public decimal TotalAmount { get; set; }

        // Snapshot fields - lưu thông tin tại thời điểm đặt hàng

        /// <summary>
        /// Tên sản phẩm (snapshot)
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// SKU (snapshot)
        /// </summary>
        public string SKU { get; set; } = string.Empty;

        /// <summary>
        /// Thông tin variant options (snapshot, JSON)
        /// </summary>
        public string? VariantOptions { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Notes { get; set; }

        // Navigation properties
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
        public ProductVariant ProductVariant { get; set; } = null!;
        public ICollection<GoodsReceiptItem> GoodsReceiptItems { get; set; } = new List<GoodsReceiptItem>();
    }
}
