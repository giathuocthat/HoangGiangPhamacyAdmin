using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class Order : AuditableEntity
    {
        // Id inherited
        public string OrderNumber { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        
        // CreatedDate inherited (replaces OrderDate)
        public string OrderStatus { get; set; } = string.Empty; // Pending, Confirmed, Shipping, Completed, Cancelled
        public string PaymentStatus { get; set; } = string.Empty; // Pending, Paid, Failed, Unpaid
        public string PaymentMethod { get; set; } = string.Empty; // Cash, VNPAY
        
        // Pricing snapshots
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;
        public decimal TotalAmount { get; set; }
        
        // Shipping Info Snapshot
        public string ShippingName { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string ShippingPhone { get; set; } = string.Empty;

        // Delivery Information
        public string DeliveryStatus { get; set; } = "NotShipped"; // NotShipped, Preparing, ReadyToShip, InTransit, OutForDelivery, Delivered, Failed, Returned
        public string? DeliveryMethod { get; set; }
        public string? ShippingCarrier { get; set; } // e.g., "Viettel Post", "Giao Hang Nhanh"
        public string? TrackingNumber { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public string? DeliveryNotes { get; set; }
        public string? Note { get; set; }
        // UpdatedDate inherited

        // Location Information
        public int? WardId { get; set; }
        public int? ProvinceId { get; set; }
        
        // Sales Information
        public int? SaleUserId { get; set; }

        // Fulfillment Information
        /// <summary>
        /// Indicates whether all order items have been fully fulfilled.
        /// True when all OrderItems have QuantityPending == 0.
        /// Null for orders that haven't been processed yet.
        /// </summary>
        public bool? IsFulfilled { get; set; }

        public Customer? Customer { get; set; }
        public Ward? Ward { get; set; }
        public Province? Province { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; }
    }
}
