using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Lịch sử thay đổi trạng thái đơn đặt hàng
    /// </summary>
    public class PurchaseOrderHistory : AuditableEntity
    {
        // Id inherited from AuditableEntity

        /// <summary>
        /// Đơn đặt hàng
        /// </summary>
        public int PurchaseOrderId { get; set; }

        /// <summary>
        /// Trạng thái trước khi thay đổi
        /// </summary>
        public string? FromStatus { get; set; }

        /// <summary>
        /// Trạng thái sau khi thay đổi
        /// </summary>
        public string ToStatus { get; set; } = string.Empty;

        /// <summary>
        /// Hành động (Created, Updated, Approved, Rejected, Cancelled, Received)
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Người thực hiện thay đổi
        /// </summary>
        public int ChangedByUserId { get; set; }

        /// <summary>
        /// Thời gian thay đổi
        /// </summary>
        public DateTime ChangedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Chi tiết thay đổi (JSON)
        /// </summary>
        public string? ChangeDetails { get; set; }

        /// <summary>
        /// Lý do thay đổi
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Notes { get; set; }

        // Navigation properties
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
    }
}
