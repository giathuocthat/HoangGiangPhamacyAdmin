using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    /// <summary>
    /// Service interface for Banner Analytics business logic
    /// </summary>
    public interface IBannerAnalyticsService
    {
        // Event Tracking
        Task TrackEventAsync(TrackBannerEventDto dto);

        // Statistics
        Task<BannerStatsDto> GetBannerStatsAsync(int bannerId, DateTime? from = null, DateTime? to = null);
        Task<IEnumerable<TopBannerDto>> GetTopBannersByViewsAsync(int count = 10, DateTime? from = null, DateTime? to = null);
        Task<IEnumerable<TopBannerDto>> GetTopBannersByClicksAsync(int count = 10, DateTime? from = null, DateTime? to = null);
        Task<IEnumerable<TopBannerDto>> GetTopBannersByCTRAsync(int count = 10, DateTime? from = null, DateTime? to = null);

        // Device Analytics
        Task<Dictionary<string, int>> GetDeviceBreakdownAsync(int bannerId, EventType eventType, DateTime? from = null, DateTime? to = null);
    }
}
