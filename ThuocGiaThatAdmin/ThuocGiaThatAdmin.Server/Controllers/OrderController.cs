using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Service.Services;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    /// <summary>
    /// Controller for Order management
    /// </summary>
    public class OrderController : BaseApiController
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService, ILogger<OrderController> logger)
            : base(logger)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        /// <summary>
        /// Customer places order from Ecommerce site
        /// </summary>
        /// <param name="dto">Customer order data</param>
        /// <returns>Created order</returns>
        [HttpPost("customer/place-order")]
        public async Task<IActionResult> CustomerPlaceOrder([FromBody] CustomerPlaceOrderDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var order = await _orderService.CustomerPlaceOrderAsync(dto);
                return Created(order, "Order placed successfully");
            }, "Customer Place Order");
        }

        /// <summary>
        /// Admin creates order from Admin panel
        /// </summary>
        /// <param name="dto">Admin order creation data</param>
        /// <returns>Created order</returns>
        [HttpPost("admin/create-order")]
        public async Task<IActionResult> AdminCreateOrder([FromBody] AdminCreateOrderDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var order = await _orderService.AdminCreateOrderAsync(dto);
                return Created(order, "Order created successfully by admin");
            }, "Admin Create Order");
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFoundResponse($"Order with ID {id} not found");

                return Success(order, "Order retrieved successfully");
            }, "Get Order");
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns>List of all orders</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            return await ExecuteActionAsync(async () =>
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Success(orders, "Orders retrieved successfully");
            }, "Get All Orders");
        }

        /// <summary>
        /// Get orders with pagination and search
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <param name="searchText">Search by phone, email, or order number</param>
        /// <returns>Paginated list of orders</returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetOrders(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchText = null)
        {
            return await ExecuteActionAsync(async () =>
            {
                if (pageSize > 100)
                {
                    return BadRequestResponse("Page size cannot exceed 100");
                }

                var (orders, totalCount) = await _orderService.GetOrdersAsync(pageNumber, pageSize, searchText);

                var response = new
                {
                    Data = orders,
                    Pagination = new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                    }
                };

                return Success(response, "Orders retrieved successfully");
            }, "Get Orders List");
        }

        /// <summary>
        /// Update order status
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <param name="dto">Update status DTO</param>
        /// <returns>Updated order</returns>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, dto.NewStatus);
                return Success(order, "Order status updated successfully");
            }, "Update Order Status");
        }

        /// <summary>
        /// Update order delivery status
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <param name="dto">Update delivery status DTO</param>
        /// <returns>Updated order</returns>
        [HttpPut("{id}/delivery-status")]
        public async Task<IActionResult> UpdateDeliveryStatus(int id, [FromBody] UpdateOrderDeliveryStatusDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var order = await _orderService.UpdateDeliveryStatusAsync(id, dto);
                return Success(order, "Order delivery status updated successfully");
            }, "Update Delivery Status");
        }
    }
}
