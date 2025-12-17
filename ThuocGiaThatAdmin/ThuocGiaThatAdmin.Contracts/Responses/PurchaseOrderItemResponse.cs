using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Contract.Responses
{
    public class PurchaseOrderItemResponse
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ProductVariantId { get; set; }
        public int OrderedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxRate { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string? VariantOptions { get; set; }
        public string? Notes { get; set; }

        // Calculated fields
        public int RemainingQuantity { get; set; }
        public decimal ReceivedPercentage { get; set; }
    }
}
