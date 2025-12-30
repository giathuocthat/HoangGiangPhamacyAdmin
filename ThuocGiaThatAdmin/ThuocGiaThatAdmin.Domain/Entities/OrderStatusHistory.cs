using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Lịch sử thay đổi trạng thái đơn hàng
    /// </summary>
    public class OrderStatusHistory : AuditableEntity
    {
        // Id inherited from AuditableEntity

        /// <summary>
        /// Đơn hàng
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Trạng thái trước khi thay đổi
        /// </summary>
        public string? OldStatus { get; set; }

        /// <summary>
        /// Trạng thái sau khi thay đổi
        /// </summary>
        public string NewStatus { get; set; } = string.Empty;

        /// <summary>
        /// Người thực hiện thay đổi
        /// </summary>
        public string ChangedByUserId { get; set; }

        /// <summary>
        /// Thời gian thay đổi
        /// </summary>
        public DateTime ChangedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Ghi chú về thay đổi
        /// </summary>
        public string? Notes { get; set; }

        // Navigation properties
        public Order Order { get; set; } = null!;
        public ApplicationUser ChangedByUser { get; set; } = null!;
    }
}
