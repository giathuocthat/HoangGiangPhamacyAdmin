using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Server.Extensions;
using ThuocGiaThatAdmin.Service.Interfaces;
using ThuocGiaThatAdmin.Service.Services;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    /// <summary>
    /// Controller for Order management
    /// </summary>
    public class OrderController : BaseApiController
    {
        private readonly OrderService _orderService;
        private readonly IAddressService _addressSevice;
        private readonly VNPayService _vnPayService;

        public OrderController(OrderService orderService, ILogger<OrderController> logger,
            IAddressService addressService, VNPayService vnPayService)
            : base(logger)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _addressSevice = addressService ?? throw new ArgumentNullException(nameof(addressService));
            _vnPayService = vnPayService ?? throw new ArgumentNullException(nameof(_vnPayService));
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
        /// Get orders with pagination and search
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <param name="searchText">Search by phone, email, or order number</param>
        /// <returns>Paginated list of orders</returns>
        [HttpGet("list")]
        [Authorize(Roles = "Customer")]
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

                var (orders, totalCount) = await _orderService.GetOrdersAsync(pageNumber, pageSize, searchText, User.GetCustomerId());

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
        /// Get orders for the logged-in customer
        /// </summary>
        ////[Authorize(Roles = "Customer")]
        [HttpGet("customer/list")]
        public async Task<IActionResult> GetCustomerOrder(
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

                // Get customer ID from claims for security
                var customerIdClaim = User.FindFirst("sub");
                if (customerIdClaim == null || !int.TryParse(customerIdClaim.Value, out int customerId))
                {
                    return UnauthorizedResponse("Invalid customer authentication");
                }

                var (orders, totalCount) =
                    await _orderService.GetOrdersAsync(pageNumber, pageSize, searchText, customerId);

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

                return Success(response, "Customer orders retrieved successfully");
            }, "Get Customer Orders");
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

        //[Authorize(Roles = "Customer")]
        [HttpGet("defaultAddress")]
        public async Task<IActionResult> GetDefaultAddress()
        {
            var customerId = User.GetCustomerId(); //int.Parse(User.FindFirst("customer_id")?.Value ?? "0");
            var address = await _addressSevice.GetDefaultAddress(customerId);
            return Ok(address);
        }

        //[Authorize(Roles = "Customer")]
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutOrderDto order)
        {
            return await ExecuteActionAsync(async () =>
            {
                var customerId = User.GetCustomerId();
                order.CustomerId = customerId;
                order.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var result = await _orderService.CreateOrderAsync(order);

                return Success<dynamic>(result, "Create Order Sucessfully");
            }, "Create Order Sucessfully");
        }

        [HttpGet("summary/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var order = await _orderService.GetOrderSummary(id);
                return Success(order, "Get Order Summary successfully");
            }, "Get Order Summary");
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

        //[Authorize(Roles = "Customer")]
        [HttpGet("listorders")]
        public async Task<IActionResult> GetListOrders()
        {
            var customerId = int.Parse(User.FindFirst("customer_id")?.Value ?? "0");
            var result = await _orderService.GetListOrders(customerId);
            return Ok(result);
        }

        //[Authorize(Roles = "Customer")]
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetOrderDetail(int id)
        {
            var customerId = int.Parse(User.FindFirst("customer_id")?.Value ?? "0");
            var result = await _orderService.GetOrderDetailAsync(id);
            return Ok(result);
        }

        [HttpGet("ipn")]
        [AllowAnonymous]
        public async Task VNPayIPN()
        {
            var rawData = await FormatRequestData(Request);


            var vnpParams = new SortedList<string, string>();
            foreach (var key in Request.Query.Keys)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpParams.Add(key, Request.Query[key]);
                }
            }

            string vnpHash = Request.Query["vnp_SecureHash"];

            await _vnPayService.UpdatePayIPN(vnpParams, rawData);
        }

        private async Task<string> FormatRequestData(HttpRequest request)
        {
            // Format raw request data for logging
            return $"{request.Method} {request.Path} {request.QueryString}";
        }


        [HttpPost("vnpay-return")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyVNPayReturn([FromBody] VNPayReturnDto vnpayDto)
        {
            try
            {
                dynamic result = await _vnPayService.VerifyVNPayReturn(vnpayDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error verifying VNPay return");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error verifying VNPay return"
                });
            }
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("invoice")]
        public async Task<IActionResult> GetInvoiceAsync()
        {
            return Ok();
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("TotalItemsInCart")]
        public async Task<IActionResult> GetTotalItemsInCartAsync()
        {
            var total = await _orderService.GetTotalItemsInCart(User.GetCustomerId());
            return Ok(total);
        }

        [HttpGet("estimated-delivery")]
        public async Task<IActionResult> GetEstimatedDelivery([FromQuery] int provinceId, [FromQuery] int wardId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var result = await _orderService.GetEstimatedDeliveryTime(provinceId, wardId);
                return Success(result, "Get Estimated Delivery Time successfully");
            }, "Get Estimated Delivery Time");
        }
    }
}