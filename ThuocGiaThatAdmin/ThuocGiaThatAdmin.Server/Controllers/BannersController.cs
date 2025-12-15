using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    /// <summary>
    /// API Controller for Banner management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BannersController : BaseApiController
    {
        private readonly IBannerService _bannerService;

        public BannersController(
            IBannerService bannerService,
            ILogger<BannersController> logger) : base(logger)
        {
            _bannerService = bannerService ?? throw new ArgumentNullException(nameof(bannerService));
        }

        /// <summary>
        /// Get all banners
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteActionAsync(async () =>
            {
                var banners = await _bannerService.GetAllAsync();
                return Success(banners);
            }, "Get All Banners");
        }

        /// <summary>
        /// Get banner by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var banner = await _bannerService.GetByIdAsync(id);
                if (banner == null)
                    return NotFoundResponse($"Banner with ID {id} not found");

                return Success(banner);
            }, "Get Banner By Id");
        }

        /// <summary>
        /// Get banner by code
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            return await ExecuteActionAsync(async () =>
            {
                var banner = await _bannerService.GetByCodeAsync(code);
                if (banner == null)
                    return NotFoundResponse($"Banner with code '{code}' not found");

                return Success(banner);
            }, "Get Banner By Code");
        }

        /// <summary>
        /// Get all active banners
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            return await ExecuteActionAsync(async () =>
            {
                var banners = await _bannerService.GetActiveBannersAsync();
                return Success(banners);
            }, "Get Active Banners");
        }

        /// <summary>
        /// Get banner slider
        /// </summary>
        [HttpGet("slider")]
        public async Task<IActionResult> GetSlider([FromQuery] int? campaignId = null, [FromQuery] int maxCount = 8)
        {
            return await ExecuteActionAsync(async () =>
            {
                var banners = await _bannerService.GetBannerSliderAsync(campaignId, maxCount);
                return Success(banners);
            }, "Get Banner Slider");
        }

        /// <summary>
        /// Get banners by campaign
        /// </summary>
        [HttpGet("campaign/{campaignId}")]
        public async Task<IActionResult> GetByCampaign(int campaignId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var banners = await _bannerService.GetByCampaignAsync(campaignId);
                return Success(banners);
            }, "Get Banners By Campaign");
        }

        /// <summary>
        /// Get paged banners
        /// </summary>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (banners, totalCount) = await _bannerService.GetPagedBannersAsync(pageNumber, pageSize);

                var response = new
                {
                    Data = banners,
                    Pagination = new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                    }
                };

                return Success(response);
            }, "Get Paged Banners");
        }

        /// <summary>
        /// Create a new banner
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBannerDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var createdBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var banner = await _bannerService.CreateAsync(dto, createdBy);
                return Success(banner, "Banner created successfully");
            }, "Create Banner");
        }

        /// <summary>
        /// Update an existing banner
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBannerDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var updatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var banner = await _bannerService.UpdateAsync(id, dto, updatedBy);
                return Success(banner, "Banner updated successfully");
            }, "Update Banner");
        }

        /// <summary>
        /// Delete a banner
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _bannerService.DeleteAsync(id);
                return Success(new { id }, "Banner deleted successfully");
            }, "Delete Banner");
        }

        /// <summary>
        /// Track banner view
        /// </summary>
        [HttpPost("{id}/track-view")]
        public async Task<IActionResult> TrackView(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _bannerService.TrackViewAsync(id);
                return Success(new { bannerId = id }, "View tracked successfully");
            }, "Track Banner View");
        }

        /// <summary>
        /// Track banner click
        /// </summary>
        [HttpPost("{id}/track-click")]
        public async Task<IActionResult> TrackClick(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _bannerService.TrackClickAsync(id);
                return Success(new { bannerId = id }, "Click tracked successfully");
            }, "Track Banner Click");
        }
    }
}
