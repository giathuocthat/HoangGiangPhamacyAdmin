using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class CustomerPaymentAccount : AuditableEntity
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public PaymentAccountType AccountType { get; set; } = PaymentAccountType.BankAccount;

        // Bank Account Info
        public int? BankId { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountHolder { get; set; } = string.Empty;
        public string? BankBranch { get; set; }
        public string? SwiftCode { get; set; }
        
        // Status
        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;

        // Notes
        public string? Notes { get; set; }

        // Navigation properties
        public Bank? Bank { get; set; }
    }
}
