using System;
using System.Collections.Generic;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Đơn đặt hàng nhập kho - Purchase Order
    /// </summary>
    public class PurchaseOrder : AuditableEntity
    {
        // Id inherited from AuditableEntity

        /// <summary>
        /// Mã đơn hàng (auto-generated, format: PO-YYYYMMDD-XXXX)
        /// </summary>
        public string OrderNumber { get; set; } = string.Empty;

        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public int SupplierId { get; set; }

        /// <summary>
        /// Người liên hệ của nhà cung cấp
        /// </summary>
        public int? SupplierContactId { get; set; }

        /// <summary>
        /// Kho nhận hàng
        /// </summary>
        public int WarehouseId { get; set; }

        /// <summary>
        /// Trạng thái đơn hàng
        /// </summary>
        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;

        /// <summary>
        /// Tổng tiền hàng (chưa thuế, chưa phí)
        /// </summary>
        public decimal SubTotal { get; set; }

        /// <summary>
        /// Tiền thuế
        /// </summary>
        public decimal TaxAmount { get; set; } = 0;

        /// <summary>
        /// Tiền giảm giá
        /// </summary>
        public decimal DiscountAmount { get; set; } = 0;

        /// <summary>
        /// Phí vận chuyển
        /// </summary>
        public decimal ShippingFee { get; set; } = 0;

        /// <summary>
        /// Tổng tiền thanh toán
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Ngày đặt hàng
        /// </summary>
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Ngày dự kiến giao hàng
        /// </summary>
        public DateTime? ExpectedDeliveryDate { get; set; }

        /// <summary>
        /// Ngày hoàn thành
        /// </summary>
        public DateTime? CompletedDate { get; set; }

        /// <summary>
        /// Người tạo đơn
        /// </summary>
        public int CreatedByUserId { get; set; }

        /// <summary>
        /// Người duyệt đơn
        /// </summary>
        public int? ApprovedByUserId { get; set; }

        /// <summary>
        /// Ngày duyệt đơn
        /// </summary>
        public DateTime? ApprovedDate { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Điều khoản đặt hàng
        /// </summary>
        public string? Terms { get; set; }

        // Navigation properties
        public Supplier Supplier { get; set; } = null!;
        public SupplierContact? SupplierContact { get; set; }
        public Warehouse Warehouse { get; set; } = null!;
        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
        public ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();
        public ICollection<PurchaseOrderHistory> PurchaseOrderHistories { get; set; } = new List<PurchaseOrderHistory>();
    }
}
