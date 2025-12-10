namespace ThuocGiaThatAdmin.Contract.Models
{
    public class PaymentInformationModel
    {
        public string OrderId { get; set; } // Mã đơn hàng (OrderCode)
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string BankCode { get; set; }
    }
}
