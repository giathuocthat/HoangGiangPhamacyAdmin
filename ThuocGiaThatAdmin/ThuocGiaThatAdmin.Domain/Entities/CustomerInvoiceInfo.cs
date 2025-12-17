using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Customer Invoice Info - Thông tin xuất hóa đơn của khách hàng
    /// Một khách hàng có thể có nhiều thông tin xuất hóa đơn khác nhau
    /// </summary>
    public class CustomerInvoiceInfo : AuditableEntity
    {
        public int CustomerId { get; set; }

        // Invoice Information
        public string BuyerName { get; set; } = string.Empty; // Tên người mua
        public string CompanyName { get; set; } = string.Empty; // Tên công ty/quầy thuốc
        public string TaxCode { get; set; } = string.Empty; // Mã số thuế
        public string InvoiceAddress { get; set; } = string.Empty; // Địa chỉ xuất hóa đơn
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Customer Customer { get; set; } = null!;
    }
}
