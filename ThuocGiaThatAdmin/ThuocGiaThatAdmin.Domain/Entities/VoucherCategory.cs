using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Liên kết voucher với danh mục sản phẩm
    /// </summary>
    public class VoucherCategory : BaseEntity
    {
        // Id inherited from BaseEntity

        /// <summary>
        /// ID của voucher
        /// </summary>
        public int VoucherId { get; set; }

        /// <summary>
        /// ID của danh mục
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Ngày tạo liên kết
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        /// <summary>
        /// Voucher
        /// </summary>
        public Voucher Voucher { get; set; } = null!;

        /// <summary>
        /// Danh mục sản phẩm
        /// </summary>
        public Category Category { get; set; } = null!;
    }
}
