using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    public class CustomerDocumentDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public CustomerDocumentType DocumentType { get; set; }
        public string DocumentTypeName { get; set; } = string.Empty;
        
        // File information
        public int UploadedFileId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        
        // Document details
        public string? DocumentNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? IssuingAuthority { get; set; }
        public string? Notes { get; set; }

        public int? ProvinceId { get; set; }
        
        // Verification status
        public bool? IsVerified { get; set; }
        public string? VerifiedByUserId { get; set; }
        public string? VerifiedByUserName { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public string? RejectionReason { get; set; }
        public bool IsRequired { get; set; }
        
        public DateTime CreatedDate { get; set; }
    }

    public class UploadCustomerDocumentDto
    {
        [Required]
        public CustomerDocumentType DocumentType { get; set; }
        
        [Required]
        public IFormFile File { get; set; } = null!;
        
        public string? DocumentNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? IssuingAuthority { get; set; }
        public string? Notes { get; set; }
        
        [Required]
        public bool IsRequired { get; set; } = false;
    }

    public class VerifyDocumentDto
    {
        [Required]
        public bool IsApproved { get; set; }
        
        public string? RejectionReason { get; set; }
        
        public string? Notes { get; set; }
    }
    
    public class UpdateCustomerDocumentDto
    {
        public string? DocumentNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? IssuingAuthority { get; set; }
        public string? Notes { get; set; }
    }
}
