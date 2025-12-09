using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    public class CustomerApprovalDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public CustomerStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        
        // Business info
        public string? CompanyName { get; set; }
        public string? TaxCode { get; set; }
        public string? BusinessRegistrationNumber { get; set; }
        
        // Documents summary
        public int TotalDocuments { get; set; }
        public int VerifiedDocuments { get; set; }
        public int RejectedDocuments { get; set; }
        public int PendingDocuments { get; set; }
        public int RequiredDocuments { get; set; }
        public int RequiredVerifiedDocuments { get; set; }
        
        // Approval info
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovedByUserName { get; set; }
        
        public DateTime CreatedDate { get; set; }
    }

    public class CustomerDetailDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public CustomerStatus Status { get; set; }
        public bool IsLoginEnabled { get; set; }
        
        // Business info
        public string? CompanyName { get; set; }
        public string? TaxCode { get; set; }
        public string? BusinessRegistrationNumber { get; set; }
        public DateTime? BusinessRegistrationDate { get; set; }
        public string? LegalRepresentative { get; set; }
        public string? BusinessAddress { get; set; }
        public string? BusinessPhone { get; set; }
        public string? BusinessEmail { get; set; }
        
        // Documents
        public List<CustomerDocumentDto> Documents { get; set; } = new();
        
        // Verification history
        public List<CustomerVerificationDto> VerificationHistory { get; set; } = new();
        
        // Approval info
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovedByUserName { get; set; }
        
        public DateTime CreatedDate { get; set; }
    }

    public class ApproveCustomerDto
    {
        [Required]
        public bool IsApproved { get; set; }
        
        public string? Notes { get; set; }
        
        public string? RejectionReason { get; set; }
    }

    public class CustomerVerificationDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public CustomerStatus OldStatus { get; set; }
        public string OldStatusName { get; set; } = string.Empty;
        public CustomerStatus NewStatus { get; set; }
        public string NewStatusName { get; set; } = string.Empty;
        public string? ProcessedByUserId { get; set; }
        public string? ProcessedByUserName { get; set; }
        public DateTime ProcessedDate { get; set; }
        public string? Notes { get; set; }
        public string? RejectionReason { get; set; }
        public bool IsInitialApproval { get; set; }
    }
}
