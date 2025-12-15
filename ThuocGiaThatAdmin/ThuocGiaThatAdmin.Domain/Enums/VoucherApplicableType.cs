using System;

namespace ThuocGiaThatAdmin.Domain.Enums
{
    /// <summary>
    /// Phạm vi áp dụng của voucher
    /// </summary>
    public enum VoucherApplicableType
    {
        /// <summary>
        /// Áp dụng cho tất cả sản phẩm
        /// </summary>
        All = 0,

        /// <summary>
        /// Áp dụng cho danh mục cụ thể
        /// </summary>
        Categories = 1,

        /// <summary>
        /// Áp dụng cho biến thể sản phẩm cụ thể 
        /// </summary>
        ProductVariants = 2,

        /// <summary>
        /// Áp dụng cho cả danh mục và biến thể sản phẩm
        /// </summary>
        Mixed = 3
    }
}
