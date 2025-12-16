using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class Customer : AuditableEntity
    {
        // ========== Basic Information ==========
        // Id inherited
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        
        // ========== Account Status & Verification ==========
        /// <summary>
        /// Trạng thái tài khoản khách hàng
        /// </summary>
        public CustomerStatus Status { get; set; } = CustomerStatus.PendingApproval;
        
        /// <summary>
        /// Có được phép đăng nhập không
        /// </summary>
        public bool IsLoginEnabled { get; set; } = true;
        
        /// <summary>
        /// Ngày được approve (khi nhân viên chủ động approve)
        /// </summary>
        public DateTime? ApprovedDate { get; set; }
        
        /// <summary>
        /// ID nhân viên approve khách hàng
        /// </summary>
        public string? ApprovedByUserId { get; set; }
        
        // ========== Business Type Classification ==========
        public int? BusinessTypeId { get; set; }                // Nullable - chỉ cho khách hàng doanh nghiệp
        public BusinessType? BusinessType { get; set; }
        
        // ========== Enterprise Information ==========
        // Chỉ áp dụng khi BusinessTypeId != null
        public string? CompanyName { get; set; }                // Tên công ty
        public string? TaxCode { get; set; }                    // Mã số thuế
        public string? BusinessRegistrationNumber { get; set; } // Số đăng ký kinh doanh
        public DateTime? BusinessRegistrationDate { get; set; } // Ngày cấp ĐKKD
        public string? LegalRepresentative { get; set; }        // Người đại diện pháp luật
        public string? BusinessLicenseUrl { get; set; }         // URL giấy phép kinh doanh (deprecated - dùng CustomerDocument)
        public string? BusinessAddress { get; set; }            // Địa chỉ kinh doanh
        public string? BusinessPhone { get; set; }              // SĐT công ty
        public string? BusinessEmail { get; set; }              // Email công ty

        public string? Otp { get; set; }
        public bool IsVerified { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public bool IsVerifiedBusiness { get; set; } = false;
        
        // ========== Sales Information ==========
        public int? SaleUserId { get; set; }                 // Nhân viên sale phụ trách
        public int? RewardPoints { get; set; } = 0;          // Điểm thưởng
        
        // ========== Navigation Properties ==========
        // Dates inherited
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<CustomerPaymentAccount> PaymentAccounts { get; set; } = new List<CustomerPaymentAccount>();
        public ICollection<CustomerDocument> Documents { get; set; } = new List<CustomerDocument>();
        public ICollection<CustomerVerification> VerificationHistory { get; set; } = new List<CustomerVerification>();
        public ApplicationUser? ApprovedByUser { get; set; }
    }
}
