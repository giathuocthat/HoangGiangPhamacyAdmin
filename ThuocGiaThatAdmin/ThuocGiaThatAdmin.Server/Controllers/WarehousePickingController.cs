using System;
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
    /// Controller cho warehouse picking operations
    /// </summary>
    [Authorize(Roles = "Admin,WarehouseManager,WarehouseStaff")]
    public class WarehousePickingController : BaseApiController
    {
        private readonly IWarehousePickingService _pickingService;

        public WarehousePickingController(
            IWarehousePickingService pickingService,
            ILogger<WarehousePickingController> logger) : base(logger)
        {
            _pickingService = pickingService;
        }

        /// <summary>
        /// Xử lý batch picking movements - chuyển hàng từ nhiều vị trí sang thùng hàng
        /// </summary>
        /// <param name="request">Request chứa danh sách movements</param>
        /// <returns>Kết quả xử lý với danh sách successful/failed movements</returns>
        [HttpPost("process")]
        public async Task<IActionResult> ProcessPicking([FromBody] ProcessPickingRequestDto request)
        {
            return await ExecuteActionAsync(async () =>
            {
                // Lấy UserId từ claims
                //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                //if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                //{
                //    return UnauthorizedResponse("User ID not found in token");
                //}

                //Logger.LogInformation($"User {userId} initiating picking process for warehouse {request.WarehouseId}");

                var result = await _pickingService.ProcessPickingAsync(request, Guid.Parse("d82e0c7d-0d76-48d7-a466-901dfe81ecac"));

                return Success(result, "Picking process completed");
            }, "Process Picking");
        }

        /// <summary>
        /// Validate location code và batch number
        /// </summary>
        /// <param name="locationCode">Mã vị trí</param>
        /// <param name="batchNumber">Số lô</param>
        /// <param name="warehouseId">ID kho</param>
        /// <returns>Thông tin validation kèm product details</returns>
        [HttpGet("validate")]
        public async Task<IActionResult> ValidatePicking(
            [FromQuery] string locationCode,
            [FromQuery] string batchNumber,
            [FromQuery] int warehouseId)
        {
            return await ExecuteActionAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(locationCode))
                {
                    return BadRequestResponse("Location code is required");
                }

                if (string.IsNullOrWhiteSpace(batchNumber))
                {
                    return BadRequestResponse("Batch number is required");
                }

                if (warehouseId <= 0)
                {
                    return BadRequestResponse("Valid warehouse ID is required");
                }

                Logger.LogInformation($"Validating location {locationCode}, batch {batchNumber} in warehouse {warehouseId}");

                var result = await _pickingService.ValidatePickingAsync(locationCode, batchNumber, warehouseId);

                return Success(result, result.IsValid ? "Validation successful" : "Validation failed");
            }, "Validate Picking");
        }
    }
}
