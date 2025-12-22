using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO for customer placing order from Ecommerce site
    /// </summary>
    public class CustomerPlaceOrderDto
    {
        // Customer Information - always required for ecommerce
        [Required(ErrorMessage = "Customer name is required")]
        public string CustomerFullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Customer email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Customer phone is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Order items are required")]
        [MinLength(1, ErrorMessage = "At least one order item is required")]
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();

        [Required(ErrorMessage = "Payment method is required")]
        public string PaymentMethod { get; set; } = string.Empty;

        public decimal ShippingFee { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;

        // Shipping Information
        [Required(ErrorMessage = "Shipping name is required")]
        public string ShippingName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Shipping address is required")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Shipping phone is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string ShippingPhone { get; set; } = string.Empty;

        // Location Codes
        public int? WardId { get; set; }
        public int? ProvinceId { get; set; }
        public string? Note { get; set; }
    }

    /// <summary>
    /// DTO for admin creating order from Admin panel
    /// </summary>
    public class AdminCreateOrderDto
    {
        // Customer Information - can select existing or create new
        public int? CustomerId { get; set; }

        // Customer details for new customer creation (required if CustomerId is null)
        public string? CustomerFullName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }

        [Required(ErrorMessage = "Order items are required")]
        [MinLength(1, ErrorMessage = "At least one order item is required")]
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();

        [Required(ErrorMessage = "Payment method is required")]
        public string PaymentMethod { get; set; } = string.Empty;

        public decimal ShippingFee { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;

        // Shipping Information
        [Required(ErrorMessage = "Shipping name is required")]
        public string ShippingName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Shipping address is required")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Shipping phone is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string ShippingPhone { get; set; } = string.Empty;

        // Location Codes
        public int? WardId { get; set; }
        public int? ProvinceId { get; set; }
        public int? CountryId { get; set; }

        public string? Note { get; set; }

        // Admin can set initial status
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
    }

    /// <summary>
    /// DTO for order item within an order - only product variant and quantity needed
    /// </summary>
    public class OrderItemDto
    {
        [Required(ErrorMessage = "Product variant ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Product variant ID must be greater than 0")]
        public int ProductVariantId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    /// <summary>
    /// DTO for order response
    /// </summary>
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public DateTime CreatedDate { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingName { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string ShippingPhone { get; set; } = string.Empty;
        public int? WardId { get; set; }
        public string? WardName { get; set; }
        public int? ProvinceId { get; set; }
        public string? ProvinceName { get; set; }
        public int? CountryId { get; set; }
        public string? CountryName { get; set; }
        public string DeliveryStatus { get; set; } = "NotShipped";
        public string? ShippingCarrier { get; set; }
        public string? TrackingNumber { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public string? DeliveryMethod { get; set; }
        public string? DeliveryNotes { get; set; }
        public string? Note { get; set; }
        public List<OrderItemResponseDto> OrderItems { get; set; } = new List<OrderItemResponseDto>();
    }

    /// <summary>
    /// DTO for order item response
    /// </summary>
    public class OrderItemResponseDto
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string VariantSKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalLineAmount { get; set; }
    }

    /// <summary>
    /// DTO for order list (simplified version for listing)
    /// </summary>
    public class OrderListDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public DateTime CreatedDate { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public bool IsFulfilled { get; set; }
    }

    /// <summary>
    /// DTO for updating order status
    /// </summary>
    public class UpdateOrderStatusDto
    {
        public string NewStatus { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for updating order delivery status
    /// </summary>
    public class UpdateOrderDeliveryStatusDto
    {
        [Required(ErrorMessage = "New delivery status is required")]
        public string NewDeliveryStatus { get; set; } = string.Empty;

        // Optional fields that can be updated with status change
        public string? ShippingCarrier { get; set; }
        public string? TrackingNumber { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string? DeliveryNotes { get; set; }
    }
}
