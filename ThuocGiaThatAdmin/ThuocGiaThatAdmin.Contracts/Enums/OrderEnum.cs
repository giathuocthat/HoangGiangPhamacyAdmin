using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Contract.Enums
{
    public enum OrderPaymentStatus
    {
        Unpaid,
        Paid,
        Failed,
        Pending
    }

    public enum PaymentMethod
    {
        [Description("cash")]
        Cash,
        CreditCard,
        DebitCard
    }

    public enum PaymentTransactionStatus
    {
        Pending = 0,
        Success,
        Failed
    }
}
