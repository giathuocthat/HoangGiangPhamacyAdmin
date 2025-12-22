using System.ComponentModel;

namespace ThuocGiaThatAdmin.Domain.Enums
{
    public enum DeliveryMethod
    {
        [Description("Giao hàng tiêu chuẩn")]
        Standard = 0,

        [Description("Giao hàng nhanh")]
        Express = 1
    }
}
