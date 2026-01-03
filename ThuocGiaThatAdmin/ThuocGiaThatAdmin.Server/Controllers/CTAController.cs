using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    /// <summary>
    /// API Controller for CTA (Call To Action) management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CTAController : BaseApiController
    {
        private readonly ICTAService _ctaService;

        public CTAController(
            ICTAService ctaService,
            ILogger<CTAController> logger) : base(logger)
        {
            _ctaService = ctaService ?? throw new ArgumentNullException(nameof(ctaService));
        }

        /// <summary>
        /// Get all CTAs
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteActionAsync(async () =>
            {
                var ctas = await _ctaService.GetAllAsync();
                return Success(ctas);
            }, "Get All CTAs");
        }

        /// <summary>
        /// Get CTA by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var cta = await _ctaService.GetByIdAsync(id);
                if (cta == null)
                    return NotFoundResponse($"CTA with ID {id} not found");

                return Success(cta);
            }, "Get CTA By Id");
        }

        /// <summary>
        /// Get CTA by code
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            return await ExecuteActionAsync(async () =>
            {
                var cta = await _ctaService.GetByCodeAsync(code);
                if (cta == null)
                    return NotFoundResponse($"CTA with code '{code}' not found");

                return Success(cta);
            }, "Get CTA By Code");
        }

        /// <summary>
        /// Get all active CTAs
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            return await ExecuteActionAsync(async () =>
            {
                var ctas = await _ctaService.GetActiveAsync();
                return Success(ctas);
            }, "Get Active CTAs");
        }

        /// <summary>
        /// Get CTAs by position
        /// </summary>
        [HttpGet("position/{position}")]
        public async Task<IActionResult> GetByPosition(CTAPosition position)
        {
            return await ExecuteActionAsync(async () =>
            {
                var ctas = await _ctaService.GetByPositionAsync(position);
                return Success(ctas);
            }, "Get CTAs By Position");
        }

        /// <summary>
        /// Get CTAs by type
        /// </summary>
        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetByType(CTAType type)
        {
            return await ExecuteActionAsync(async () =>
            {
                var ctas = await _ctaService.GetByTypeAsync(type);
                return Success(ctas);
            }, "Get CTAs By Type");
        }

        /// <summary>
        /// Get CTAs by campaign
        /// </summary>
        [HttpGet("campaign/{campaignId}")]
        public async Task<IActionResult> GetByCampaign(int campaignId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var ctas = await _ctaService.GetByCampaignAsync(campaignId);
                return Success(ctas);
            }, "Get CTAs By Campaign");
        }

        /// <summary>
        /// Get paged CTAs
        /// </summary>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (ctas, totalCount) = await _ctaService.GetPagedAsync(pageNumber, pageSize);

                var response = new
                {
                    Data = ctas,
                    Pagination = new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                    }
                };

                return Success(response);
            }, "Get Paged CTAs");
        }

        /// <summary>
        /// Create a new CTA
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCTADto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var createdBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var cta = await _ctaService.CreateAsync(dto, createdBy);
                return Success(cta, "CTA created successfully");
            }, "Create CTA");
        }

        /// <summary>
        /// Update an existing CTA
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCTADto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var updatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var cta = await _ctaService.UpdateAsync(id, dto, updatedBy);
                return Success(cta, "CTA updated successfully");
            }, "Update CTA");
        }

        /// <summary>
        /// Delete a CTA
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _ctaService.DeleteAsync(id);
                return Success(new { id }, "CTA deleted successfully");
            }, "Delete CTA");
        }

        /// <summary>
        /// Track CTA view
        /// </summary>
        [HttpPost("{id}/track-view")]
        public async Task<IActionResult> TrackView(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _ctaService.TrackViewAsync(id);
                return Success(new { ctaId = id }, "View tracked successfully");
            }, "Track CTA View");
        }

        /// <summary>
        /// Track CTA click
        /// </summary>
        [HttpPost("{id}/track-click")]
        public async Task<IActionResult> TrackClick(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _ctaService.TrackClickAsync(id);
                return Success(new { ctaId = id }, "Click tracked successfully");
            }, "Track CTA Click");
        }
    }
}
