using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository interface for BannerAnalytics entity
    /// </summary>
    public interface IBannerAnalyticsRepository : IRepository<BannerAnalytics>
    {
        /// <summary>
        /// Track banner event (view or click)
        /// </summary>
        Task TrackEventAsync(BannerAnalytics analytics);

        /// <summary>
        /// Get analytics for a specific banner
        /// </summary>
        Task<IEnumerable<BannerAnalytics>> GetByBannerAsync(int bannerId, DateTime? from = null, DateTime? to = null);

        /// <summary>
        /// Get banner statistics (aggregated data)
        /// </summary>
        Task<(int views, int clicks, decimal ctr)> GetBannerStatsAsync(int bannerId, DateTime? from = null, DateTime? to = null);

        /// <summary>
        /// Get top performing banners by views
        /// </summary>
        Task<IEnumerable<(int bannerId, int viewCount, int clickCount)>> GetTopBannersByViewsAsync(int count = 10, DateTime? from = null, DateTime? to = null);

        /// <summary>
        /// Get top performing banners by clicks
        /// </summary>
        Task<IEnumerable<(int bannerId, int viewCount, int clickCount)>> GetTopBannersByClicksAsync(int count = 10, DateTime? from = null, DateTime? to = null);

        /// <summary>
        /// Get top performing banners by CTR
        /// </summary>
        Task<IEnumerable<(int bannerId, int viewCount, int clickCount, decimal ctr)>> GetTopBannersByCTRAsync(int count = 10, DateTime? from = null, DateTime? to = null);

        /// <summary>
        /// Get device breakdown for a banner
        /// </summary>
        Task<Dictionary<string, int>> GetDeviceBreakdownAsync(int bannerId, EventType eventType, DateTime? from = null, DateTime? to = null);

        /// <summary>
        /// Get unique viewers count
        /// </summary>
        Task<int> GetUniqueViewersCountAsync(int bannerId, DateTime? from = null, DateTime? to = null);

        /// <summary>
        /// Get unique clickers count
        /// </summary>
        Task<int> GetUniqueClickersCountAsync(int bannerId, DateTime? from = null, DateTime? to = null);
    }
}
