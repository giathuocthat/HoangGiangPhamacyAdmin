using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Domain.Enums;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service.Services
{
    /// <summary>
    /// Service for Banner Analytics business logic
    /// </summary>
    public class BannerAnalyticsService : IBannerAnalyticsService
    {
        private readonly IBannerAnalyticsRepository _analyticsRepository;
        private readonly IBannerRepository _bannerRepository;

        public BannerAnalyticsService(
            IBannerAnalyticsRepository analyticsRepository,
            IBannerRepository bannerRepository)
        {
            _analyticsRepository = analyticsRepository ?? throw new ArgumentNullException(nameof(analyticsRepository));
            _bannerRepository = bannerRepository ?? throw new ArgumentNullException(nameof(bannerRepository));
        }

        #region Event Tracking

        public async Task TrackEventAsync(TrackBannerEventDto dto)
        {
            var analytics = new BannerAnalytics
            {
                BannerId = dto.BannerId,
                CustomerId = dto.CustomerId,
                IpAddress = dto.IpAddress,
                UserAgent = dto.UserAgent,
                DeviceType = dto.DeviceType,
                EventType = (int)dto.EventType
            };

            await _analyticsRepository.TrackEventAsync(analytics);
            await _analyticsRepository.SaveChangesAsync();
        }

        #endregion

        #region Statistics

        public async Task<BannerStatsDto> GetBannerStatsAsync(int bannerId, DateTime? from = null, DateTime? to = null)
        {
            var banner = await _bannerRepository.GetByIdAsync(bannerId);
            if (banner == null)
                throw new InvalidOperationException($"Banner with ID {bannerId} not found");

            var (views, clicks, ctr) = await _analyticsRepository.GetBannerStatsAsync(bannerId, from, to);
            var uniqueViewers = await _analyticsRepository.GetUniqueViewersCountAsync(bannerId, from, to);
            var uniqueClickers = await _analyticsRepository.GetUniqueClickersCountAsync(bannerId, from, to);

            // Get device breakdown for views
            var deviceBreakdown = await _analyticsRepository.GetDeviceBreakdownAsync(bannerId, EventType.View, from, to);

            return new BannerStatsDto
            {
                BannerId = bannerId,
                BannerTitle = banner.Title ?? string.Empty,
                TotalViews = views,
                TotalClicks = clicks,
                ClickThroughRate = ctr,
                UniqueViewers = uniqueViewers,
                UniqueClickers = uniqueClickers,
                DesktopViews = deviceBreakdown.GetValueOrDefault("Desktop", 0),
                MobileViews = deviceBreakdown.GetValueOrDefault("Mobile", 0),
                TabletViews = deviceBreakdown.GetValueOrDefault("Tablet", 0),
                FromDate = from,
                ToDate = to
            };
        }

        public async Task<IEnumerable<TopBannerDto>> GetTopBannersByViewsAsync(int count = 10, DateTime? from = null, DateTime? to = null)
        {
            var topBanners = await _analyticsRepository.GetTopBannersByViewsAsync(count, from, to);
            var result = new List<TopBannerDto>();
            var rank = 1;

            foreach (var (bannerId, viewCount, clickCount) in topBanners)
            {
                var banner = await _bannerRepository.GetByIdAsync(bannerId);
                if (banner != null)
                {
                    var ctr = viewCount > 0 ? (decimal)clickCount / viewCount * 100 : 0;
                    result.Add(new TopBannerDto
                    {
                        BannerId = bannerId,
                        BannerCode = banner.BannerCode ?? string.Empty,
                        BannerTitle = banner.Title ?? string.Empty,
                        ViewCount = viewCount,
                        ClickCount = clickCount,
                        ClickThroughRate = ctr,
                        Rank = rank++
                    });
                }
            }

            return result;
        }

        public async Task<IEnumerable<TopBannerDto>> GetTopBannersByClicksAsync(int count = 10, DateTime? from = null, DateTime? to = null)
        {
            var topBanners = await _analyticsRepository.GetTopBannersByClicksAsync(count, from, to);
            var result = new List<TopBannerDto>();
            var rank = 1;

            foreach (var (bannerId, viewCount, clickCount) in topBanners)
            {
                var banner = await _bannerRepository.GetByIdAsync(bannerId);
                if (banner != null)
                {
                    var ctr = viewCount > 0 ? (decimal)clickCount / viewCount * 100 : 0;
                    result.Add(new TopBannerDto
                    {
                        BannerId = bannerId,
                        BannerCode = banner.BannerCode ?? string.Empty,
                        BannerTitle = banner.Title ?? string.Empty,
                        ViewCount = viewCount,
                        ClickCount = clickCount,
                        ClickThroughRate = ctr,
                        Rank = rank++
                    });
                }
            }

            return result;
        }

        public async Task<IEnumerable<TopBannerDto>> GetTopBannersByCTRAsync(int count = 10, DateTime? from = null, DateTime? to = null)
        {
            var topBanners = await _analyticsRepository.GetTopBannersByCTRAsync(count, from, to);
            var result = new List<TopBannerDto>();
            var rank = 1;

            foreach (var (bannerId, viewCount, clickCount, ctr) in topBanners)
            {
                var banner = await _bannerRepository.GetByIdAsync(bannerId);
                if (banner != null)
                {
                    result.Add(new TopBannerDto
                    {
                        BannerId = bannerId,
                        BannerCode = banner.BannerCode ?? string.Empty,
                        BannerTitle = banner.Title ?? string.Empty,
                        ViewCount = viewCount,
                        ClickCount = clickCount,
                        ClickThroughRate = ctr,
                        Rank = rank++
                    });
                }
            }

            return result;
        }

        #endregion

        #region Device Analytics

        public async Task<Dictionary<string, int>> GetDeviceBreakdownAsync(int bannerId, EventType eventType, DateTime? from = null, DateTime? to = null)
        {
            var banner = await _bannerRepository.GetByIdAsync(bannerId);
            if (banner == null)
                throw new InvalidOperationException($"Banner with ID {bannerId} not found");

            return await _analyticsRepository.GetDeviceBreakdownAsync(bannerId, eventType, from, to);
        }

        #endregion
    }
}
