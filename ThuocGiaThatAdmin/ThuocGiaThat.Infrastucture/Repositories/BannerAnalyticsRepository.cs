using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository implementation for BannerAnalytics entity
    /// </summary>
    public class BannerAnalyticsRepository : Repository<BannerAnalytics>, IBannerAnalyticsRepository
    {
        public BannerAnalyticsRepository(TrueMecContext context) : base(context)
        {
        }

        /// <summary>
        /// Track banner event
        /// </summary>
        public async Task TrackEventAsync(BannerAnalytics analytics)
        {
            if (analytics == null)
                throw new ArgumentNullException(nameof(analytics));

            await _dbSet.AddAsync(analytics);
        }

        /// <summary>
        /// Get analytics for a specific banner
        /// </summary>
        public async Task<IEnumerable<BannerAnalytics>> GetByBannerAsync(int bannerId, DateTime? from = null, DateTime? to = null)
        {
            if (bannerId <= 0)
                throw new ArgumentException("Banner ID must be greater than 0", nameof(bannerId));

            var query = _dbSet.Where(a => a.BannerId == bannerId);

            if (from.HasValue)
                query = query.Where(a => a.CreatedDate >= from.Value);

            if (to.HasValue)
                query = query.Where(a => a.CreatedDate <= to.Value);

            return await query
                .OrderByDescending(a => a.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Get banner statistics
        /// </summary>
        public async Task<(int views, int clicks, decimal ctr)> GetBannerStatsAsync(int bannerId, DateTime? from = null, DateTime? to = null)
        {
            if (bannerId <= 0)
                throw new ArgumentException("Banner ID must be greater than 0", nameof(bannerId));

            var query = _dbSet.Where(a => a.BannerId == bannerId);

            if (from.HasValue)
                query = query.Where(a => a.CreatedDate >= from.Value);

            if (to.HasValue)
                query = query.Where(a => a.CreatedDate <= to.Value);

            var views = await query.CountAsync(a => a.EventType == (int)EventType.View);
            var clicks = await query.CountAsync(a => a.EventType == (int)EventType.Click);
            var ctr = views > 0 ? (decimal)clicks / views * 100 : 0;

            return (views, clicks, ctr);
        }

        /// <summary>
        /// Get top performing banners by views
        /// </summary>
        public async Task<IEnumerable<(int bannerId, int viewCount, int clickCount)>> GetTopBannersByViewsAsync(int count = 10, DateTime? from = null, DateTime? to = null)
        {
            var query = _dbSet.AsQueryable();

            if (from.HasValue)
                query = query.Where(a => a.CreatedDate >= from.Value);

            if (to.HasValue)
                query = query.Where(a => a.CreatedDate <= to.Value);

            return await query
                .GroupBy(a => a.BannerId)
                .Select(g => new
                {
                    BannerId = g.Key,
                    ViewCount = g.Count(a => a.EventType == (int)EventType.View),
                    ClickCount = g.Count(a => a.EventType == (int)EventType.Click)
                })
                .OrderByDescending(x => x.ViewCount)
                .Take(count)
                .Select(x => ValueTuple.Create(x.BannerId, x.ViewCount, x.ClickCount))
                .ToListAsync();
        }

        /// <summary>
        /// Get top performing banners by clicks
        /// </summary>
        public async Task<IEnumerable<(int bannerId, int viewCount, int clickCount)>> GetTopBannersByClicksAsync(int count = 10, DateTime? from = null, DateTime? to = null)
        {
            var query = _dbSet.AsQueryable();

            if (from.HasValue)
                query = query.Where(a => a.CreatedDate >= from.Value);

            if (to.HasValue)
                query = query.Where(a => a.CreatedDate <= to.Value);

            return await query
                .GroupBy(a => a.BannerId)
                .Select(g => new
                {
                    BannerId = g.Key,
                    ViewCount = g.Count(a => a.EventType == (int)EventType.View),
                    ClickCount = g.Count(a => a.EventType == (int)EventType.Click)
                })
                .OrderByDescending(x => x.ClickCount)
                .Take(count)
                .Select(x => ValueTuple.Create(x.BannerId, x.ViewCount, x.ClickCount))
                .ToListAsync();
        }

        /// <summary>
        /// Get top performing banners by CTR
        /// </summary>
        public async Task<IEnumerable<(int bannerId, int viewCount, int clickCount, decimal ctr)>> GetTopBannersByCTRAsync(int count = 10, DateTime? from = null, DateTime? to = null)
        {
            var query = _dbSet.AsQueryable();

            if (from.HasValue)
                query = query.Where(a => a.CreatedDate >= from.Value);

            if (to.HasValue)
                query = query.Where(a => a.CreatedDate <= to.Value);

            var stats = await query
                .GroupBy(a => a.BannerId)
                .Select(g => new
                {
                    BannerId = g.Key,
                    ViewCount = g.Count(a => a.EventType == (int)EventType.View),
                    ClickCount = g.Count(a => a.EventType == (int)EventType.Click)
                })
                .ToListAsync();

            return stats
                .Select(s => (
                    bannerId: s.BannerId,
                    viewCount: s.ViewCount,
                    clickCount: s.ClickCount,
                    ctr: s.ViewCount > 0 ? (decimal)s.ClickCount / s.ViewCount * 100 : 0
                ))
                .OrderByDescending(x => x.ctr)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// Get device breakdown
        /// </summary>
        public async Task<Dictionary<string, int>> GetDeviceBreakdownAsync(int bannerId, EventType eventType, DateTime? from = null, DateTime? to = null)
        {
            if (bannerId <= 0)
                throw new ArgumentException("Banner ID must be greater than 0", nameof(bannerId));

            var query = _dbSet
                .Where(a => a.BannerId == bannerId && a.EventType == (int)eventType);

            // Note: BannerAnalytics doesn't have CreatedAt, date filtering not supported
            // if (from.HasValue)
            //     query = query.Where(a => a.CreatedAt >= from.Value);

            // if (to.HasValue)
            //     query = query.Where(a => a.CreatedAt <= to.Value);

            return await query
                .GroupBy(a => a.DeviceType ?? "Unknown")
                .Select(g => new { DeviceType = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.DeviceType, x => x.Count);
        }

        /// <summary>
        /// Get unique viewers count
        /// </summary>
        public async Task<int> GetUniqueViewersCountAsync(int bannerId, DateTime? from = null, DateTime? to = null)
        {
            if (bannerId <= 0)
                throw new ArgumentException("Banner ID must be greater than 0", nameof(bannerId));

            var query = _dbSet
                .Where(a => a.BannerId == bannerId && a.EventType == (int)EventType.View);

            // Note: BannerAnalytics doesn't have CreatedAt, date filtering not supported
            // if (from.HasValue)
            //     query = query.Where(a => a.CreatedAt >= from.Value);

            // if (to.HasValue)
            //     query = query.Where(a => a.CreatedAt <= to.Value);

            // Count unique by CustomerId (if logged in) or IpAddress (if anonymous)
            return await query
                .Select(a => a.CustomerId.HasValue ? a.CustomerId.ToString() : a.IpAddress)
                .Distinct()
                .CountAsync();
        }

        /// <summary>
        /// Get unique clickers count
        /// </summary>
        public async Task<int> GetUniqueClickersCountAsync(int bannerId, DateTime? from = null, DateTime? to = null)
        {
            if (bannerId <= 0)
                throw new ArgumentException("Banner ID must be greater than 0", nameof(bannerId));

            var query = _dbSet
                .Where(a => a.BannerId == bannerId && a.EventType == (int)EventType.Click);

            // Note: BannerAnalytics doesn't have CreatedAt, date filtering not supported
            // if (from.HasValue)
            //     query = query.Where(a => a.CreatedAt >= from.Value);

            // if (to.HasValue)
            //     query = query.Where(a => a.CreatedAt <= to.Value);

            // Count unique by CustomerId (if logged in) or IpAddress (if anonymous)
            return await query
                .Select(a => a.CustomerId.HasValue ? a.CustomerId.ToString() : a.IpAddress)
                .Distinct()
                .CountAsync();
        }
    }
}
