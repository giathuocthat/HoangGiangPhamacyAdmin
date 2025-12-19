using System;
using System.ComponentModel.DataAnnotations;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO for verifying/rejecting a customer
    /// </summary>
    public class VerifyCustomerDto
    {
        /// <summary>
        /// True = Approve, False = Reject/Suspend
        /// </summary>
        [Required]
        public bool IsApproved { get; set; }
        
        /// <summary>
        /// Rejection reason (required if IsApproved = false)
        /// </summary>
        public string? RejectionReason { get; set; }
        
        /// <summary>
        /// Additional notes about the verification
        /// </summary>
        public string? Notes { get; set; }
        
        /// <summary>
        /// Rating for the customer profile (1-5)
        /// </summary>
        public int? Rating { get; set; }
    }

    /// <summary>
    /// Response DTO for customer verification record
    /// </summary>
    public class CustomerVerificationResponseDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public CustomerStatus OldStatus { get; set; }
        public string OldStatusName { get; set; } = string.Empty;
        public CustomerStatus NewStatus { get; set; }
        public string NewStatusName { get; set; } = string.Empty;
        public string? ProcessedByUserId { get; set; }
        public string? ProcessedByUserName { get; set; }
        public DateTime ProcessedDate { get; set; }
        public string? Notes { get; set; }
        public string? RejectionReason { get; set; }
        public int? Rating { get; set; }
        public bool IsInitialApproval { get; set; }
    }

    /// <summary>
    /// Customer status DTO with verification details
    /// </summary>
    public class CustomerStatusDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public CustomerStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovedByUserId { get; set; }
        public string? ApprovedByUserName { get; set; }
        public int VerifiedDocumentsCount { get; set; }
        public int TotalDocumentsCount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
