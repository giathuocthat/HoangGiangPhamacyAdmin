using System.ComponentModel;

namespace ThuocGiaThatAdmin.Domain.Enums
{
    /// <summary>
    /// Order status enumeration
    /// Workflow: Pending -> Confirmed -> Processing -> InTransit -> Shipping -> Completed
    /// Any status can transition to Cancelled
    /// </summary>
    public enum OrderStatus
    {
        [Description("Chờ xác nhận")]
        Pending,
        [Description("Đã xác nhận")]
        Confirmed = 1,
        [Description("Đang xử lý")]
        Processing = 2,
        [Description("Đã chuẩn bị hàng")]
        ReadyToShip = 3,
        [Description("Đang trung chuyển")]
        InTransit = 4,      // Đang trung chuyển
        [Description("Đang giao hàng")]
        Shipping = 5,
        [Description("Đã giao")]
        Delivered = 6,
        [Description("Đã hoàn tất")]
        Completed = 7,
        [Description("Đang xử lý hủy")]
        Cancelling = 8,
        [Description("Đã hủy")]
        Cancelled = 9,
    }

    /// <summary>
    /// Extension methods for OrderStatus
    /// </summary>
    public static class OrderStatusExtensions
    {
        /// <summary>
        /// Get valid next statuses based on current status
        /// </summary>
        public static OrderStatus[] GetValidNextStatuses(this OrderStatus currentStatus)
        {
            return currentStatus switch
            {
                OrderStatus.Pending => new[] { OrderStatus.Confirmed, OrderStatus.Cancelled },
                OrderStatus.Confirmed => new[] { OrderStatus.Processing, OrderStatus.Cancelled },
                OrderStatus.Processing => new[] { OrderStatus.InTransit, OrderStatus.Cancelled },
                OrderStatus.InTransit => new[] { OrderStatus.Shipping, OrderStatus.Cancelled },
                OrderStatus.Shipping => new[] { OrderStatus.Delivered, OrderStatus.Cancelled },
                OrderStatus.Delivered => new[] { OrderStatus.Completed },
                OrderStatus.Completed => new OrderStatus[] { }, // No transitions from Completed
                OrderStatus.Cancelled => new OrderStatus[] { }, // No transitions from Cancelled
                _ => new OrderStatus[] { }
            };
        }

        /// <summary>
        /// Check if status transition is valid
        /// </summary>
        public static bool CanTransitionTo(this OrderStatus currentStatus, OrderStatus newStatus)
        {
            var validStatuses = currentStatus.GetValidNextStatuses();
            return Array.Exists(validStatuses, s => s == newStatus);
        }

        /// <summary>
        /// Convert enum to string
        /// </summary>
        public static string ToStatusString(this OrderStatus status)
        {
            return status.ToString();
        }

        /// <summary>
        /// Parse string to OrderStatus enum
        /// </summary>
        public static OrderStatus ParseStatus(string statusString)
        {
            if (Enum.TryParse<OrderStatus>(statusString, true, out var result))
            {
                return result;
            }
            throw new ArgumentException($"Invalid order status: {statusString}");
        }
    }
}
