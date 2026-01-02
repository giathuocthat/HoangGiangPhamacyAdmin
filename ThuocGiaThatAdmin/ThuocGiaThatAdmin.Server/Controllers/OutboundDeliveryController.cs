using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Server.Extensions;
using ThuocGiaThatAdmin.Service.Services;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    /// <summary>
    /// Controller cho xuất kho giao hàng (Outbound Delivery/Shipment)
    /// </summary>
    //[Authorize(Roles = "Admin,WarehouseManager,WarehouseStaff")]
    public class OutboundDeliveryController : BaseApiController
    {
        private readonly OrderService _orderService;

        public OutboundDeliveryController(
            OrderService orderService,
            ILogger<OutboundDeliveryController> logger) : base(logger)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Lấy thông tin chi tiết đơn hàng để chuẩn bị xuất kho
        /// </summary>
        /// <param name="orderNumber">Mã đơn hàng (OrderNumber)</param>
        /// <returns>Chi tiết đơn hàng</returns>
        [HttpGet("order-details/{orderNumber}")]
        public async Task<IActionResult> GetOrderDetails(string orderNumber)
        {
            return await ExecuteActionAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(orderNumber))
                {
                    return BadRequestResponse("Order number is required");
                }

                var result = await _orderService.GetOrderDetailsForShipmentAsync(orderNumber);

                if (result == null)
                {
                    return NotFoundResponse($"Order {orderNumber} not found");
                }

                return Success(result, "Order details retrieved successfully");
            }, "Get Order Details for Shipment");
        }

        /// <summary>
        /// Xử lý xuất kho giao hàng
        /// - Trừ inventory quantity
        /// - Trừ reserved quantity
        /// - Chuyển order status sang InTransit
        /// - Clear warehouse location (container)
        /// </summary>
        /// <param name="request">Shipment request</param>
        /// <returns>Kết quả shipment</returns>
        [HttpPost("process-shipment")]
        public async Task<IActionResult> ProcessShipment([FromBody] ProcessShipmentRequestDto request)
        {
            return await ExecuteActionAsync(async () =>
            {
                var result = await _orderService.ProcessShipmentAsync(request, User.GetUserId());

                return Success(result, result.Success ? "Shipment processed successfully" : "Shipment processing failed");
            }, "Process Shipment");
        }
    }
}
