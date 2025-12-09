using System;

namespace ThuocGiaThatAdmin.Domain.Enums
{
    /// <summary>
    /// Loại điều kiện số lượng tối thiểu
    /// </summary>
    public enum MinimumQuantityType
    {
        /// <summary>
        /// Tổng số lượng sản phẩm (quantity)
        /// </summary>
        TotalQuantity = 0,

        /// <summary>
        /// Số loại sản phẩm khác nhau (distinct products)
        /// </summary>
        DistinctProducts = 1
    }
}
