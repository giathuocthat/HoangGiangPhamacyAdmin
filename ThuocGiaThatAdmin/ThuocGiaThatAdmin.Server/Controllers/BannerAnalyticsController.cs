using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Enums;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    /// <summary>
    /// API Controller for Banner Analytics
    /// </summary>
    [ApiController]
    [Route("api/banner-analytics")]
    public class BannerAnalyticsController : BaseApiController
    {
        private readonly IBannerAnalyticsService _analyticsService;

        public BannerAnalyticsController(
            IBannerAnalyticsService analyticsService,
            ILogger<BannerAnalyticsController> logger) : base(logger)
        {
            _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
        }

        /// <summary>
        /// Track banner event (view or click)
        /// </summary>
        [HttpPost("track")]
        public async Task<IActionResult> TrackEvent([FromBody] TrackBannerEventDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _analyticsService.TrackEventAsync(dto);
                return Success(new { bannerId = dto.BannerId, eventType = dto.EventType }, "Event tracked successfully");
            }, "Track Banner Event");
        }

        /// <summary>
        /// Get banner statistics
        /// </summary>
        [HttpGet("{bannerId}/stats")]
        public async Task<IActionResult> GetStats(
            int bannerId,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            return await ExecuteActionAsync(async () =>
            {
                var stats = await _analyticsService.GetBannerStatsAsync(bannerId, from, to);
                return Success(stats);
            }, "Get Banner Stats");
        }

        /// <summary>
        /// Get top banners by views
        /// </summary>
        [HttpGet("top-by-views")]
        public async Task<IActionResult> GetTopByViews(
            [FromQuery] int count = 10,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            return await ExecuteActionAsync(async () =>
            {
                var topBanners = await _analyticsService.GetTopBannersByViewsAsync(count, from, to);
                return Success(topBanners);
            }, "Get Top Banners By Views");
        }

        /// <summary>
        /// Get top banners by clicks
        /// </summary>
        [HttpGet("top-by-clicks")]
        public async Task<IActionResult> GetTopByClicks(
            [FromQuery] int count = 10,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            return await ExecuteActionAsync(async () =>
            {
                var topBanners = await _analyticsService.GetTopBannersByClicksAsync(count, from, to);
                return Success(topBanners);
            }, "Get Top Banners By Clicks");
        }

        /// <summary>
        /// Get top banners by CTR (Click-Through Rate)
        /// </summary>
        [HttpGet("top-by-ctr")]
        public async Task<IActionResult> GetTopByCTR(
            [FromQuery] int count = 10,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            return await ExecuteActionAsync(async () =>
            {
                var topBanners = await _analyticsService.GetTopBannersByCTRAsync(count, from, to);
                return Success(topBanners);
            }, "Get Top Banners By CTR");
        }

        /// <summary>
        /// Get device breakdown for banner events
        /// </summary>
        [HttpGet("{bannerId}/device-breakdown")]
        public async Task<IActionResult> GetDeviceBreakdown(
            int bannerId,
            [FromQuery] EventType eventType = EventType.View,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            return await ExecuteActionAsync(async () =>
            {
                var breakdown = await _analyticsService.GetDeviceBreakdownAsync(bannerId, eventType, from, to);
                return Success(breakdown);
            }, "Get Device Breakdown");
        }
    }
}
