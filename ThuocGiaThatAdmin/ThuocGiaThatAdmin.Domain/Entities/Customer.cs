using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class Customer : AuditableEntity
    {
        // ========== Basic Information ==========
        // Id inherited
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        
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
        public string? BusinessLicenseUrl { get; set; }         // URL giấy phép kinh doanh
        public string? BusinessAddress { get; set; }            // Địa chỉ kinh doanh
        public string? BusinessPhone { get; set; }              // SĐT công ty
        public string? BusinessEmail { get; set; }              // Email công ty
        
        // ========== Navigation Properties ==========
        // Dates inherited
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<CustomerPaymentAccount> PaymentAccounts { get; set; } = new List<CustomerPaymentAccount>();
    }
}
