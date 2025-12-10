namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class PaymentTransaction : AuditableEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public string? TransactionCode { get; set; }
        public string? VNPAYTransactionNo { get; set; }
        public string? BankCode { get; set; }

        public decimal Amount { get; set; }

        public string PaymentGateway { get; set; } = "VNPAY";

        public int PaymentStatus { get; set; }

        public string? ResponseCode { get; set; }
        public string? Message { get; set; }

        public DateTimeOffset? PayDate { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }

        public string? CardType { get; set; }
        public string? BankTranNo { get; set; }
    }
}
