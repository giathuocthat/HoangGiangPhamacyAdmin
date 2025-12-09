using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Liên kết nhiều voucher với một đơn hàng (hỗ trợ stacking)
    /// </summary>
    public class OrderVoucher : BaseEntity
    {
        // Id inherited from BaseEntity

        /// <summary>
        /// ID của đơn hàng
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// ID của voucher
        /// </summary>
        public int VoucherId { get; set; }

        /// <summary>
        /// Thứ tự áp dụng voucher (1, 2, 3...)
        /// </summary>
        public int AppliedOrder { get; set; }

        /// <summary>
        /// Số tiền giảm của voucher này
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        /// <summary>
        /// Đơn hàng
        /// </summary>
        public Order Order { get; set; } = null!;

        /// <summary>
        /// Voucher
        /// </summary>
        public Voucher Voucher { get; set; } = null!;
    }
}
