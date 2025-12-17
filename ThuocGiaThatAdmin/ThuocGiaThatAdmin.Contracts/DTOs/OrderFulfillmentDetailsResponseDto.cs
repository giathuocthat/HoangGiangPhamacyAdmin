using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// Response DTO cho order fulfillment details (for warehouse picking)
    /// </summary>
    public class OrderFulfillmentDetailsResponseDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public List<OrderItemWithFulfillmentDto> Items { get; set; } = new();
    }
}
