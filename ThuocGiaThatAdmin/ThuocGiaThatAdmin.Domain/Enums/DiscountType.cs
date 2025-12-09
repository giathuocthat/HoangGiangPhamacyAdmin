using System;

namespace ThuocGiaThatAdmin.Domain.Enums
{
    /// <summary>
    /// Loại giảm giá của voucher
    /// </summary>
    public enum DiscountType
    {
        /// <summary>
        /// Giảm theo phần trăm
        /// </summary>
        Percentage = 0,

        /// <summary>
        /// Giảm số tiền cố định
        /// </summary>
        FixedAmount = 1,

        /// <summary>
        /// Miễn phí vận chuyển
        /// </summary>
        FreeShipping = 2,

        /// <summary>
        /// Mua X tặng Y
        /// </summary>
        BuyXGetY = 3
    }
}
