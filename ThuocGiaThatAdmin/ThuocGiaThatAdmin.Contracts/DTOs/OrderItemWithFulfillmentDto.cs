using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO cho order item với fulfillment details
    /// </summary>
    public class OrderItemWithFulfillmentDto
    {
        public int OrderItemId { get; set; }
        public int ProductVariantId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int QuantityOrdered { get; set; }
        public List<OrderItemFulfillmentDetailDto> Fulfillments { get; set; } = new();
    }
}
