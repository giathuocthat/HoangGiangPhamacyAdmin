using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Nhà cung cấp - Supplier
    /// </summary>
    public class Supplier : AuditableEntity
    {
        // Id inherited from AuditableEntity

        /// <summary>
        /// Mã nhà cung cấp (unique)
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Tên nhà cung cấp
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Phường/Xã
        /// </summary>
        public int? WardId { get; set; }

        /// <summary>
        /// Tỉnh/Thành phố
        /// </summary>
        public int? ProvinceId { get; set; }

        /// <summary>
        /// Mã số thuế
        /// </summary>
        public string? TaxCode { get; set; }

        /// <summary>
        /// Số tài khoản ngân hàng
        /// </summary>
        public string? BankAccount { get; set; }

        /// <summary>
        /// Tên ngân hàng
        /// </summary>
        public string? BankName { get; set; }

        /// <summary>
        /// Điều khoản thanh toán (số ngày)
        /// </summary>
        public int PaymentTerms { get; set; } = 30;

        /// <summary>
        /// Hạn mức công nợ
        /// </summary>
        public decimal? CreditLimit { get; set; }

        /// <summary>
        /// Trạng thái hoạt động
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Đánh giá nhà cung cấp (1-5 sao)
        /// </summary>
        public int? Rating { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Notes { get; set; }

        // Navigation properties
        public Ward? Ward { get; set; }
        public Province? Province { get; set; }
        public ICollection<SupplierContact> SupplierContacts { get; set; } = new List<SupplierContact>();
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}
