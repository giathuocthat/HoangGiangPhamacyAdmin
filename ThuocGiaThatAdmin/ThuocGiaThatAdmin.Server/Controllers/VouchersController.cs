using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VouchersController : BaseApiController
    {
        private readonly IVoucherService _voucherService;

        public VouchersController(IVoucherService voucherService, ILogger<VouchersController> logger) : base(logger)
        {
            _voucherService = voucherService ?? throw new ArgumentNullException(nameof(voucherService));
        }

        [HttpGet]
        public async Task<IActionResult> GetVouchers()
        {
            var result = await _voucherService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get all active vouchers
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveVouchers()
        {
            return await ExecuteActionAsync(async () =>
            {
                var vouchers = await _voucherService.GetAllActiveAsync();
                return Success(vouchers);
            }, "Get Active Vouchers");
        }

        /// <summary>
        /// Get voucher by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoucherById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var voucher = await _voucherService.GetByIdAsync(id);
                if (voucher == null)
                    return NotFoundResponse($"Voucher with ID {id} not found");

                return Success(voucher);
            }, "Get Voucher By Id");
        }

        /// <summary>
        /// Get voucher by code
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetVoucherByCode(string code)
        {
            return await ExecuteActionAsync(async () =>
            {
                var voucher = await _voucherService.GetByCodeAsync(code);
                if (voucher == null)
                    return NotFoundResponse($"Voucher with code '{code}' not found");

                return Success(voucher);
            }, "Get Voucher By Code");
        }

        /// <summary>
        /// Create a new voucher
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var createdBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var voucher = await _voucherService.CreateAsync(dto, createdBy);
                return Created(voucher, "Voucher created successfully");
            }, "Create Voucher");
        }

        /// <summary>
        /// Update an existing voucher
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVoucher(int id, [FromBody] UpdateVoucherDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var updatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var voucher = await _voucherService.UpdateAsync(id, dto, updatedBy);
                return Success(voucher, "Voucher updated successfully");
            }, "Update Voucher");
        }

        /// <summary>
        /// Delete a voucher
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVoucher(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _voucherService.DeleteAsync(id);
                return Success(new { voucherId = id }, "Voucher deleted successfully");
            }, "Delete Voucher");
        }

        /// <summary>
        /// Validate a single voucher
        /// </summary>
        [HttpPost("validate")]
        public async Task<IActionResult> ValidateVoucher([FromBody] ValidateVoucherDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var result = await _voucherService.ValidateVoucherAsync(dto);
                return Success(result);
            }, "Validate Voucher");
        }

        /// <summary>
        /// Validate multiple vouchers for stacking
        /// </summary>
        [HttpPost("validate-multiple")]
        public async Task<IActionResult> ValidateMultipleVouchers([FromBody] ValidateMultipleVouchersDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var result = await _voucherService.ValidateMultipleVouchersAsync(dto);
                return Success(result);
            }, "Validate Multiple Vouchers");
        }

        /// <summary>
        /// Calculate discount for a single voucher
        /// </summary>
        [HttpPost("calculate-discount")]
        public async Task<IActionResult> CalculateDiscount([FromBody] ValidateVoucherDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var discount = await _voucherService.CalculateDiscountAsync(dto.VoucherCode, dto.CartItems, dto.OrderSubTotal);
                return Success(new { discountAmount = discount, finalAmount = dto.OrderSubTotal - discount });
            }, "Calculate Discount");
        }

        /// <summary>
        /// Calculate stacked discounts for multiple vouchers
        /// </summary>
        [HttpPost("calculate-stacked")]
        public async Task<IActionResult> CalculateStackedDiscounts([FromBody] ValidateMultipleVouchersDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (totalDiscount, applications) = await _voucherService.CalculateStackedDiscountsAsync(
                    dto.VoucherCodes,
                    dto.CartItems,
                    dto.OrderSubTotal);

                return Success(new
                {
                    totalDiscountAmount = totalDiscount,
                    finalAmount = dto.OrderSubTotal - totalDiscount,
                    applications = applications
                });
            }, "Calculate Stacked Discounts");
        }

        /// <summary>
        /// Get available vouchers for current user
        /// </summary>
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableVouchers()
        {
            return await ExecuteActionAsync(async () =>
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var vouchers = await _voucherService.GetAvailableVouchersForUserAsync(userId);
                return Success(vouchers);
            }, "Get Available Vouchers");
        }

        /// <summary>
        /// Get stackable vouchers
        /// </summary>
        [HttpGet("stackable")]
        public async Task<IActionResult> GetStackableVouchers()
        {
            return await ExecuteActionAsync(async () =>
            {
                var vouchers = await _voucherService.GetStackableVouchersAsync();
                return Success(vouchers);
            }, "Get Stackable Vouchers");
        }

        /// <summary>
        /// Get usage history for a voucher
        /// </summary>
        [HttpGet("{id}/usage-history")]
        public async Task<IActionResult> GetUsageHistory(int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            return await ExecuteActionAsync(async () =>
            {
                var history = await _voucherService.GetUsageHistoryAsync(id, pageNumber, pageSize);
                return Success(history);
            }, "Get Voucher Usage History");
        }

        /// <summary>
        /// Get user's voucher usage history
        /// </summary>
        [HttpGet("user/usage-history")]
        public async Task<IActionResult> GetUserUsageHistory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            return await ExecuteActionAsync(async () =>
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
                var history = await _voucherService.GetUserUsageHistoryAsync(userId, pageNumber, pageSize);
                return Success(history);
            }, "Get User Usage History");
        }
    }
}
