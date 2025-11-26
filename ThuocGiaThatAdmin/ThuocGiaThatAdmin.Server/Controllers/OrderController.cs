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
    }
}
