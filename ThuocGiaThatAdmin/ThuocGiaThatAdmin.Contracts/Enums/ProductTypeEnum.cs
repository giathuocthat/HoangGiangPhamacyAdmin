using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Contract.Enums
{
    public enum ProductType
    {
        [Description("Thuốc kê đơn")]
        PrescriptionDrug = 1,

        [Description("Thuốc không kê đơn")]
        NonPrescriptionDrug = 2,

        [Description("Thực phẩm chức năng")]
        DietarySupplement = 3,

        [Description("Thiết bị y tế")]
        MedicalDevice = 4,

        [Description("Dược mỹ phẩm")]
        Cosmeceutical = 5,

        [Description("Hàng tiêu dùng")]
        ConsumerGoods = 6
    }

}
