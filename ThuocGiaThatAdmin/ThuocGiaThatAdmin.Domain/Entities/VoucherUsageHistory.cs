using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Lịch sử sử dụng voucher
    /// </summary>
    public class VoucherUsageHistory : BaseEntity
    {
        // Id inherited from BaseEntity

        /// <summary>
        /// ID của voucher
        /// </summary>
        public int VoucherId { get; set; }

        /// <summary>
        /// ID của user sử dụng
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// ID của đơn hàng
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Số tiền được giảm
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Tổng giá trị đơn hàng trước khi giảm
        /// </summary>
        public decimal OrderTotalBeforeDiscount { get; set; }

        /// <summary>
        /// Tổng giá trị đơn hàng sau khi giảm
        /// </summary>
        public decimal OrderTotalAfterDiscount { get; set; }

        /// <summary>
        /// Thời gian sử dụng
        /// </summary>
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        /// <summary>
        /// Voucher được sử dụng
        /// </summary>
        public Voucher Voucher { get; set; } = null!;

        /// <summary>
        /// User sử dụng voucher
        /// </summary>
        public ApplicationUser User { get; set; } = null!;

        /// <summary>
        /// Đơn hàng áp dụng voucher
        /// </summary>
        public Order Order { get; set; } = null!;
    }
}
