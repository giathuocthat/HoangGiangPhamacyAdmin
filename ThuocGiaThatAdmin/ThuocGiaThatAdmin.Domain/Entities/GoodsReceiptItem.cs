using System;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Chi tiết sản phẩm trong phiếu nhập kho
    /// </summary>
    public class GoodsReceiptItem : AuditableEntity
    {
        // Id inherited from AuditableEntity

        /// <summary>
        /// Phiếu nhập kho
        /// </summary>
        public int GoodsReceiptId { get; set; }

        /// <summary>
        /// Chi tiết đơn đặt hàng
        /// </summary>
        public int PurchaseOrderItemId { get; set; }

        /// <summary>
        /// Số lượng đặt hàng (từ PurchaseOrderItem)
        /// </summary>
        public int OrderedQuantity { get; set; }

        /// <summary>
        /// Số lượng thực tế nhận
        /// </summary>
        public int ReceivedQuantity { get; set; }

        /// <summary>
        /// Số lượng chấp nhận (sau kiểm tra chất lượng)
        /// </summary>
        public int AcceptedQuantity { get; set; } = 0;

        /// <summary>
        /// Trạng thái chất lượng
        /// </summary>
        public QualityStatus QualityStatus { get; set; } = QualityStatus.Good;

        /// <summary>
        /// Số lượng từ chối
        /// </summary>
        public int RejectedQuantity { get; set; } = 0;

        /// <summary>
        /// Số lô
        /// </summary>
        public string? BatchNumber { get; set; }

        /// <summary>
        /// Ngày sản xuất
        /// </summary>
        public DateTime? ManufactureDate { get; set; }

        /// <summary>
        /// Hạn sử dụng
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Mã vị trí trong kho
        /// </summary>
        public string? LocationCode { get; set; }

        /// <summary>
        /// Số kệ
        /// </summary>
        public string? ShelfNumber { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Ghi chú kiểm tra chất lượng
        /// </summary>
        public string? InspectionNotes { get; set; }

        // Navigation properties
        public GoodsReceipt GoodsReceipt { get; set; } = null!;
        public PurchaseOrderItem PurchaseOrderItem { get; set; } = null!;
    }
}
