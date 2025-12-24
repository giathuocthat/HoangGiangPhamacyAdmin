using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.Enums;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    public class CheckoutOrderDto
    {
        public decimal ShippingFee { get; set; }
        public IList<CheckoutOderItemDto> Items { get; set; }
        public int? CustomerId { get; set; }
        public string Note { get; set; }
        //public PaymentMethod PaymentMethod { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public decimal UtilityFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public string ShippingName { get; set; }
        public string ShippingPhone { get; set; }
        public string ShippingAddress { get; set; }
        public int? WardId { get; set; }
        public int? ProvinceId { get; set; }
        public string PaymentMethod { get; set; }
        public string? IpAddress { get; set; }
        public bool ExportInvoice { get; set; }
        public int? InvoiceId { get; set; }

    }

    public class CheckoutOderItemDto
    {
        public int ProductId { get; set; }
        public int ProductVariantId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
