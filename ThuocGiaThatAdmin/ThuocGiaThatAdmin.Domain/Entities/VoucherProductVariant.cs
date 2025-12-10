using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Liên kết voucher với biến thể sản phẩm
    /// </summary>
    public class VoucherProductVariant : BaseEntity
    {
        // Id inherited from BaseEntity

        /// <summary>
        /// ID của voucher
        /// </summary>
        public int VoucherId { get; set; }

        /// <summary>
        /// ID của biến thể sản phẩm
        /// </summary>
        public int ProductVariantId { get; set; }

        /// <summary>
        /// Áp dụng cho ApplicableType = ProductVariants
        /// </summary>
        public int? MinimumQuantity { get; set; }

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
        /// Biến thể sản phẩm
        /// </summary>
        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
