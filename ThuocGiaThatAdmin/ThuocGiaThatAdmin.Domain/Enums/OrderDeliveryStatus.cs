namespace ThuocGiaThatAdmin.Domain.Enums
{
    /// <summary>
    /// Order delivery status enumeration
    /// Workflow: NotShipped -> Preparing -> ReadyToShip -> InTransit -> OutForDelivery -> Delivered
    /// Can transition to Failed or Returned from certain stages
    /// </summary>
    public enum OrderDeliveryStatus
    {
        NotShipped = 0,
        Preparing = 1,
        ReadyToShip = 2,
        InTransit = 3,
        OutForDelivery = 4,
        Delivered = 5,
        Failed = 6,
        Returned = 7
    }

    /// <summary>
    /// Extension methods for OrderDeliveryStatus
    /// </summary>
    public static class OrderDeliveryStatusExtensions
    {
        /// <summary>
        /// Get valid next statuses based on current delivery status
        /// </summary>
        public static OrderDeliveryStatus[] GetValidNextStatuses(this OrderDeliveryStatus currentStatus)
        {
            return currentStatus switch
            {
                OrderDeliveryStatus.NotShipped => new[] { OrderDeliveryStatus.Preparing },
                OrderDeliveryStatus.Preparing => new[] { OrderDeliveryStatus.ReadyToShip, OrderDeliveryStatus.Failed },
                OrderDeliveryStatus.ReadyToShip => new[] { OrderDeliveryStatus.InTransit },
                OrderDeliveryStatus.InTransit => new[] { OrderDeliveryStatus.OutForDelivery, OrderDeliveryStatus.Failed, OrderDeliveryStatus.Returned },
                OrderDeliveryStatus.OutForDelivery => new[] { OrderDeliveryStatus.Delivered, OrderDeliveryStatus.Failed, OrderDeliveryStatus.Returned },
                OrderDeliveryStatus.Delivered => new OrderDeliveryStatus[] { }, // Terminal state
                OrderDeliveryStatus.Failed => new[] { OrderDeliveryStatus.Preparing }, // Can retry from Preparing
                OrderDeliveryStatus.Returned => new OrderDeliveryStatus[] { }, // Terminal state
                _ => new OrderDeliveryStatus[] { }
            };
        }

        /// <summary>
        /// Check if delivery status transition is valid
        /// </summary>
        public static bool CanTransitionTo(this OrderDeliveryStatus currentStatus, OrderDeliveryStatus newStatus)
        {
            var validStatuses = currentStatus.GetValidNextStatuses();
            return Array.Exists(validStatuses, s => s == newStatus);
        }

        /// <summary>
        /// Convert enum to string
        /// </summary>
        public static string ToStatusString(this OrderDeliveryStatus status)
        {
            return status.ToString();
        }

        /// <summary>
        /// Parse string to OrderDeliveryStatus enum
        /// </summary>
        public static OrderDeliveryStatus ParseStatus(string statusString)
        {
            if (Enum.TryParse<OrderDeliveryStatus>(statusString, true, out var result))
            {
                return result;
            }
            throw new ArgumentException($"Invalid delivery status: {statusString}");
        }
    }
}
