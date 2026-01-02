using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ThuocGiaThatAdmin.Contract.Enums
{
    public enum ProductCollectionTypeEnum
    {
        [Description("Không xác định")]
        None = 0,
        [Description("Hot")]
        Hot = 1,
        [Description("Top")]
        Top = 2,
        [Description("Nổi bật (Featured)")]
        Featured = 3,
        [Description("Yêu thích (Favorite)")]
        Favorite = 4,
        [Description("Bán chạy (BestSeller)")]
        BestSeller = 5,
        [Description("Gợi ý (Recommendation)")]
        Recommendation = 6,
        [Description("Sản phẩm mới")]
        News = 7,
        [Description("Đang khuyến mãi")]
        Sales = 8,
    }
}
