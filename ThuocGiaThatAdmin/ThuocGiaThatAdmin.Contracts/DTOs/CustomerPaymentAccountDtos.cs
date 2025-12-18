using System;
using System.ComponentModel.DataAnnotations;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO for CustomerPaymentAccount response
    /// </summary>
    public class CustomerPaymentAccountDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public PaymentAccountType AccountType { get; set; }

        // Bank Account Info
        public int? BankId { get; set; }
        public string? BankName { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountHolder { get; set; } = string.Empty;
        public string? BankBranch { get; set; }
        public string? SwiftCode { get; set; }

        // Status
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }

        // Audit
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    /// <summary>
    /// DTO for creating new CustomerPaymentAccount
    /// </summary>
    public class CreateCustomerPaymentAccountDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public PaymentAccountType AccountType { get; set; } = PaymentAccountType.BankAccount;

        // Bank Account Info
        public int? BankId { get; set; }

        [Required]
        [StringLength(100)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string AccountHolder { get; set; } = string.Empty;

        [StringLength(200)]
        public string? BankBranch { get; set; }

        [StringLength(50)]
        public string? SwiftCode { get; set; }

        public bool IsDefault { get; set; } = false;

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO for updating CustomerPaymentAccount
    /// </summary>
    public class UpdateCustomerPaymentAccountDto
    {
        [Required]
        public PaymentAccountType AccountType { get; set; }

        public int? BankId { get; set; }

        [Required]
        [StringLength(100)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string AccountHolder { get; set; } = string.Empty;

        [StringLength(200)]
        public string? BankBranch { get; set; }

        [StringLength(50)]
        public string? SwiftCode { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Lightweight DTO for list views
    /// </summary>
    public class CustomerPaymentAccountListItemDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public PaymentAccountType AccountType { get; set; }
        public string? BankName { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountHolder { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }
}
