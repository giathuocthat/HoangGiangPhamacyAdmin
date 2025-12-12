using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// Request DTO cho API fulfill đơn hàng
    /// </summary>
    public class FulfillOrderRequestDto
    {
        /// <summary>
        /// Danh sách OrderId cần fulfill
        /// Nếu null hoặc empty, service sẽ tự động chọn các đơn hàng ưu tiên (theo CreatedDate)
        /// </summary>
        public List<int>? OrderIds { get; set; }
        
        /// <summary>
        /// ID kho để lấy hàng
        /// </summary>
        public int WarehouseId { get; set; }
    }

    /// <summary>
    /// Response DTO cho API fulfill đơn hàng
    /// </summary>
    public class FulfillOrderResponseDto
    {
        /// <summary>
        /// Tổng số đơn hàng đã xử lý
        /// </summary>
        public int TotalOrdersProcessed { get; set; }
        
        /// <summary>
        /// Danh sách đơn hàng fulfill thành công hoàn toàn
        /// </summary>
        public List<OrderFulfillmentDetailDto> SuccessfulOrders { get; set; } = new();
        
        /// <summary>
        /// Danh sách đơn hàng fulfill một phần (không đủ hàng)
        /// </summary>
        public List<OrderFulfillmentDetailDto> PartialOrders { get; set; } = new();
        
        /// <summary>
        /// Danh sách đơn hàng không thể fulfill (lỗi)
        /// </summary>
        public List<OrderFulfillmentErrorDto> FailedOrders { get; set; } = new();
    }

    /// <summary>
    /// Chi tiết fulfill của một đơn hàng
    /// </summary>
    public class OrderFulfillmentDetailDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public List<OrderItemFulfillmentDto> Items { get; set; } = new();
    }

    /// <summary>
    /// Chi tiết fulfill của một OrderItem
    /// </summary>
    public class OrderItemFulfillmentDto
    {
        public int OrderItemId { get; set; }
        public int ProductVariantId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        
        /// <summary>
        /// Số lượng đã đặt
        /// </summary>
        public int QuantityOrdered { get; set; }
        
        /// <summary>
        /// Số lượng đã fulfill (bao gồm cả lần fulfill trước đó nếu có)
        /// </summary>
        public int QuantityFulfilled { get; set; }
        
        /// <summary>
        /// Số lượng còn chưa fulfill
        /// </summary>
        public int QuantityPending { get; set; }
        
        /// <summary>
        /// Danh sách lô hàng được allocate trong lần fulfill này
        /// </summary>
        public List<BatchAllocationDto> BatchAllocations { get; set; } = new();
    }

    /// <summary>
    /// Thông tin allocation từ một batch cụ thể
    /// </summary>
    public class BatchAllocationDto
    {
        public int InventoryBatchId { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        
        /// <summary>
        /// Số lượng được allocate từ batch này
        /// </summary>
        public int QuantityAllocated { get; set; }
    }

    /// <summary>
    /// Thông tin lỗi khi fulfill đơn hàng
    /// </summary>
    public class OrderFulfillmentErrorDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
