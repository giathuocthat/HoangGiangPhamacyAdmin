using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Contract.Enums
{
    public enum PaymentStatus
    {
        [Description("Chờ xác nhận")]
        WaitingConfirm,

        [Description("Đã xác nhận")]
        Confirmed,
    }

    public enum PaymentMethod
    {
        [Description("cash")]
        Cash,
        CreditCard,
        DebitCard
    }
}
