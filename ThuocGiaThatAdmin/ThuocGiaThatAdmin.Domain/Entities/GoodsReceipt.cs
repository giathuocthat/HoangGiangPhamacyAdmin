using System;
using System.Collections.Generic;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Phiếu nhập kho - Goods Receipt
    /// </summary>
    public class GoodsReceipt : AuditableEntity
    {
        // Id inherited from AuditableEntity

        /// <summary>
        /// Mã phiếu nhập kho (auto-generated, format: GR-YYYYMMDD-XXXX)
        /// </summary>
        public string ReceiptNumber { get; set; } = string.Empty;

        /// <summary>
        /// Đơn đặt hàng
        /// </summary>
        public int PurchaseOrderId { get; set; }

        /// <summary>
        /// Kho nhận hàng
        /// </summary>
        public int WarehouseId { get; set; }

        /// <summary>
        /// Trạng thái phiếu nhập
        /// </summary>
        public GoodsReceiptStatus Status { get; set; } = GoodsReceiptStatus.Pending;

        /// <summary>
        /// Ngày dự kiến nhận hàng
        /// </summary>
        public DateTime? ScheduledDate { get; set; }

        /// <summary>
        /// Ngày thực tế nhận hàng
        /// </summary>
        public DateTime? ReceivedDate { get; set; }

        /// <summary>
        /// Ngày hoàn thành nhập kho
        /// </summary>
        public DateTime? CompletedDate { get; set; }

        /// <summary>
        /// Người nhận hàng
        /// </summary>
        public int? ReceivedByUserId { get; set; }

        /// <summary>
        /// Người kiểm tra chất lượng
        /// </summary>
        public int? InspectedByUserId { get; set; }

        /// <summary>
        /// Đơn vị vận chuyển
        /// </summary>
        public string? ShippingCarrier { get; set; }

        /// <summary>
        /// Mã vận đơn
        /// </summary>
        public string? TrackingNumber { get; set; }

        /// <summary>
        /// Biển số xe
        /// </summary>
        public string? VehicleNumber { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Lý do từ chối (nếu có)
        /// </summary>
        public string? RejectionReason { get; set; }

        // Navigation properties
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
        public ICollection<GoodsReceiptItem> GoodsReceiptItems { get; set; } = new List<GoodsReceiptItem>();
    }
}
