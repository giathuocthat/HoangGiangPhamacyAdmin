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
    [Authorize(Roles = "Admin,WarehouseManager")]
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
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return UnauthorizedResponse("User ID not found in token");
                }

                Logger.LogInformation($"User {userId} initiating order fulfillment for warehouse {request.WarehouseId}");

                var result = await _fulfillmentService.FulfillOrdersAsync(request, userId);

                return Success(result, "Order fulfillment completed");
            }, "Fulfill Orders");
        }
    }
}
