using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// Request DTO cho việc xuất kho giao hàng (Outbound Delivery)
    /// </summary>
    public class ProcessShipmentRequestDto
    {
        /// <summary>
        /// Mã đơn hàng (OrderNumber) - VD: "ORD-20251219-357203"
        /// </summary>
        [Required(ErrorMessage = "Order number is required")]
        public string OrderNumber { get; set; } = string.Empty;

        /// <summary>
        /// Mã thùng hàng/container location code
        /// </summary>
        [Required(ErrorMessage = "Container location code is required")]
        public string ContainerLocationCode { get; set; } = string.Empty;

        /// <summary>
        /// ID kho
        /// </summary>
        [Required(ErrorMessage = "Warehouse ID is required")]
        public int WarehouseId { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Response DTO cho shipment
    /// </summary>
    public class ProcessShipmentResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;
        public int TotalItemsShipped { get; set; }
        public decimal TotalQuantityShipped { get; set; }
    }

    /// <summary>
    /// Response DTO cho order details để hiển thị trước khi ship
    /// </summary>
    public class ShipmentOrderDetailsDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public List<ShipmentOrderItemDto> Items { get; set; } = new();
    }

    /// <summary>
    /// Order item trong shipment
    /// </summary>
    public class ShipmentOrderItemDto
    {
        public int OrderItemId { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int QuantityFulfilled { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalLineAmount { get; set; }
    }
}
