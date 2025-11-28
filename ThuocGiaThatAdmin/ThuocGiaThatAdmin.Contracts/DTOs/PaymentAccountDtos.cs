using System.ComponentModel.DataAnnotations;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    public class CreatePaymentAccountDto
    {
        [Required]
        public PaymentAccountType AccountType { get; set; }

        [Required]
        [StringLength(100)]
        public string BankName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string AccountHolder { get; set; } = string.Empty;

        [StringLength(200)]
        public string? BankBranch { get; set; }

        [StringLength(20)]
        public string? SwiftCode { get; set; }

        public bool IsDefault { get; set; } = false;

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class UpdatePaymentAccountDto
    {
        [Required]
        [StringLength(100)]
        public string BankName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string AccountHolder { get; set; } = string.Empty;

        [StringLength(200)]
        public string? BankBranch { get; set; }

        [StringLength(20)]
        public string? SwiftCode { get; set; }

        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class PaymentAccountDto
    {
        public int Id { get; set; }
        public PaymentAccountType AccountType { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountHolder { get; set; } = string.Empty;
        public string? BankBranch { get; set; }
        public string? SwiftCode { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }
}
