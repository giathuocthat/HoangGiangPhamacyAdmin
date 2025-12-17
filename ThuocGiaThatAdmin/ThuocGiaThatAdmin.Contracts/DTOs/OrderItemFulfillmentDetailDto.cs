using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO cho fulfillment của một order item
    /// </summary>
    public class OrderItemFulfillmentDetailDto
    {
        public int FulfillmentId { get; set; }
        public int InventoryBatchId { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public int QuantityFulfilled { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string SuggestedLocation { get; set; } = string.Empty;
        public List<BatchLocationDto> AvailableLocations { get; set; } = new();
    }
}
