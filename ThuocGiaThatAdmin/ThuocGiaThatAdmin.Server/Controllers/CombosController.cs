using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    /// <summary>
    /// API Controller for Combo management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CombosController : BaseApiController
    {
        private readonly IComboService _comboService;

        public CombosController(
            IComboService comboService,
            ILogger<CombosController> logger) : base(logger)
        {
            _comboService = comboService ?? throw new ArgumentNullException(nameof(comboService));
        }

        /// <summary>
        /// Get all combos
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteActionAsync(async () =>
            {
                var combos = await _comboService.GetAllAsync();
                return Success(combos);
            }, "Get All Combos");
        }

        /// <summary>
        /// Get combo by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var combo = await _comboService.GetByIdAsync(id);
                if (combo == null)
                    return NotFoundResponse($"Combo with ID {id} not found");

                return Success(combo);
            }, "Get Combo By Id");
        }

        /// <summary>
        /// Get combo by code
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            return await ExecuteActionAsync(async () =>
            {
                var combo = await _comboService.GetByCodeAsync(code);
                if (combo == null)
                    return NotFoundResponse($"Combo with code '{code}' not found");

                return Success(combo);
            }, "Get Combo By Code");
        }

        /// <summary>
        /// Get all active combos
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            return await ExecuteActionAsync(async () =>
            {
                var combos = await _comboService.GetActiveCombosAsync();
                return Success(combos);
            }, "Get Active Combos");
        }

        /// <summary>
        /// Get combos by banner
        /// </summary>
        [HttpGet("banner/{bannerId}")]
        public async Task<IActionResult> GetByBanner(int bannerId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var combos = await _comboService.GetByBannerAsync(bannerId);
                return Success(combos);
            }, "Get Combos By Banner");
        }

        /// <summary>
        /// Get paged combos
        /// </summary>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (combos, totalCount) = await _comboService.GetPagedCombosAsync(pageNumber, pageSize);

                var response = new
                {
                    Data = combos,
                    Pagination = new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                    }
                };

                return Success(response);
            }, "Get Paged Combos");
        }

        /// <summary>
        /// Check stock availability for combo
        /// </summary>
        [HttpGet("{id}/check-stock")]
        public async Task<IActionResult> CheckStock(int id, [FromQuery] int quantity = 1)
        {
            return await ExecuteActionAsync(async () =>
            {
                var available = await _comboService.CheckStockAvailabilityAsync(id, quantity);
                return Success(new { comboId = id, quantity, available });
            }, "Check Combo Stock");
        }

        /// <summary>
        /// Create a new combo
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateComboDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var createdBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var combo = await _comboService.CreateAsync(dto, createdBy);
                return Success(combo, "Combo created successfully");
            }, "Create Combo");
        }

        /// <summary>
        /// Update an existing combo
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateComboDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var updatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var combo = await _comboService.UpdateAsync(id, dto, updatedBy);
                return Success(combo, "Combo updated successfully");
            }, "Update Combo");
        }

        /// <summary>
        /// Delete a combo
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _comboService.DeleteAsync(id);
                return Success(new { id }, "Combo deleted successfully");
            }, "Delete Combo");
        }
    }
}
