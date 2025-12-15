using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    /// <summary>
    /// API Controller for Campaign management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CampaignsController : BaseApiController
    {
        private readonly ICampaignService _campaignService;

        public CampaignsController(
            ICampaignService campaignService,
            ILogger<CampaignsController> logger) : base(logger)
        {
            _campaignService = campaignService ?? throw new ArgumentNullException(nameof(campaignService));
        }

        /// <summary>
        /// Get all campaigns
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteActionAsync(async () =>
            {
                var campaigns = await _campaignService.GetAllAsync();
                return Success(campaigns);
            }, "Get All Campaigns");
        }

        /// <summary>
        /// Get campaign by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var campaign = await _campaignService.GetByIdAsync(id);
                if (campaign == null)
                    return NotFoundResponse($"Campaign with ID {id} not found");

                return Success(campaign);
            }, "Get Campaign By Id");
        }

        /// <summary>
        /// Get campaign by code
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            return await ExecuteActionAsync(async () =>
            {
                var campaign = await _campaignService.GetByCodeAsync(code);
                if (campaign == null)
                    return NotFoundResponse($"Campaign with code '{code}' not found");

                return Success(campaign);
            }, "Get Campaign By Code");
        }

        /// <summary>
        /// Get all active campaigns
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            return await ExecuteActionAsync(async () =>
            {
                var campaigns = await _campaignService.GetActiveCampaignsAsync();
                return Success(campaigns);
            }, "Get Active Campaigns");
        }

        /// <summary>
        /// Get campaign with banners
        /// </summary>
        [HttpGet("{id}/with-banners")]
        public async Task<IActionResult> GetWithBanners(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var campaign = await _campaignService.GetWithBannersAsync(id);
                if (campaign == null)
                    return NotFoundResponse($"Campaign with ID {id} not found");

                return Success(campaign);
            }, "Get Campaign With Banners");
        }

        /// <summary>
        /// Get paged campaigns
        /// </summary>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (campaigns, totalCount) = await _campaignService.GetPagedCampaignsAsync(pageNumber, pageSize);

                var response = new
                {
                    Data = campaigns,
                    Pagination = new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                    }
                };

                return Success(response);
            }, "Get Paged Campaigns");
        }

        /// <summary>
        /// Create a new campaign
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCampaignDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var createdBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var campaign = await _campaignService.CreateAsync(dto, createdBy);
                return Success(campaign, "Campaign created successfully");
            }, "Create Campaign");
        }

        /// <summary>
        /// Update an existing campaign
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCampaignDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var updatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
                var campaign = await _campaignService.UpdateAsync(id, dto, updatedBy);
                return Success(campaign, "Campaign updated successfully");
            }, "Update Campaign");
        }

        /// <summary>
        /// Delete a campaign
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _campaignService.DeleteAsync(id);
                return Success(new { id }, "Campaign deleted successfully");
            }, "Delete Campaign");
        }
    }
}
