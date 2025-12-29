using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    /// <summary>
    /// Controller for order fulfillment operations
    /// </summary>
    //[Authorize(Roles = "Admin,WarehouseManager")]
    public class OrderFulfillmentController : BaseApiController
    {
        private readonly IOrderFulfillmentService _fulfillmentService;

        public OrderFulfillmentController(
            IOrderFulfillmentService fulfillmentService,
            ILogger<OrderFulfillmentController> logger) : base(logger)
        {
            _fulfillmentService = fulfillmentService;
        }

        /// <summary>
        /// Fulfill đơn hàng - Book sản phẩm từ kho cho đơn hàng
        /// </summary>
        /// <param name="request">Request chứa OrderIds (optional) và WarehouseId</param>
        /// <returns>Kết quả fulfill với danh sách đơn hàng successful, partial, và failed</returns>
        [HttpPost("fulfill")]
        public async Task<IActionResult> FulfillOrders([FromBody] FulfillOrderRequestDto request)
        {
            return await ExecuteActionAsync(async () =>
            {
                // Lấy UserId từ claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                userIdClaim = "cb5b42d4-cb1e-4499-a789-52be2c17300f"; // Temporary hardcode for testing
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return UnauthorizedResponse("User ID not found in token");
                }

                Logger.LogInformation($"User {userId} initiating order fulfillment for warehouse {request.WarehouseId}");

                var result = await _fulfillmentService.FulfillOrdersAsync(request, userId);

                return Success(result, "Order fulfillment completed");
            }, "Fulfill Orders");
        }

        /// <summary>
        /// Lấy order fulfillment details cho warehouse picking
        /// Bao gồm thông tin batches đã fulfill và suggested locations
        /// </summary>
        /// <param name="orderIdentifier">ID hoặc OrderNumber của Order (e.g., "123" hoặc "ORD-20251219-357203")</param>
        /// <param name="warehouseId">ID của Warehouse</param>
        /// <returns>Order fulfillment details với suggested locations</returns>
        [HttpGet("order/{orderIdentifier}")]
        public async Task<IActionResult> GetOrderFulfillmentDetails(string orderIdentifier, [FromQuery] int warehouseId)
        {
            return await ExecuteActionAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(orderIdentifier))
                {
                    return BadRequestResponse("Order identifier is required");
                }

                if (warehouseId <= 0)
                {
                    return BadRequestResponse("Invalid warehouse ID");
                }

                Logger.LogInformation($"Getting fulfillment details for order {orderIdentifier} in warehouse {warehouseId}");

                var result = await _fulfillmentService.GetOrderFulfillmentDetailsAsync(orderIdentifier, warehouseId);

                return Success(result, "Order fulfillment details retrieved successfully");
            }, "Get Order Fulfillment Details");
        }
    }
}
