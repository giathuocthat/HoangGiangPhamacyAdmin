using System;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO cho tracking banner event (view/click)
    /// </summary>
    public class TrackBannerEventDto
    {
        public int BannerId { get; set; }
        public int? CustomerId { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? DeviceType { get; set; }
        public EventType EventType { get; set; }
    }

    /// <summary>
    /// DTO cho banner analytics response
    /// </summary>
    public class BannerAnalyticsResponseDto
    {
        public int Id { get; set; }
        public int BannerId { get; set; }
        public string BannerTitle { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? DeviceType { get; set; }
        public EventType EventType { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO cho banner statistics (aggregated data)
    /// </summary>
    public class BannerStatsDto
    {
        public int BannerId { get; set; }
        public string BannerTitle { get; set; } = string.Empty;
        public int TotalViews { get; set; }
        public int TotalClicks { get; set; }
        public decimal ClickThroughRate { get; set; } // CTR percentage
        public int UniqueViewers { get; set; }
        public int UniqueClickers { get; set; }

        // Device breakdown
        public int DesktopViews { get; set; }
        public int MobileViews { get; set; }
        public int TabletViews { get; set; }

        // Time period
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    /// <summary>
    /// DTO cho top performing banners
    /// </summary>
    public class TopBannerDto
    {
        public int BannerId { get; set; }
        public string BannerCode { get; set; } = string.Empty;
        public string BannerTitle { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public int ClickCount { get; set; }
        public decimal ClickThroughRate { get; set; }
        public int Rank { get; set; }
    }

    /// <summary>
    /// DTO cho banner performance comparison
    /// </summary>
    public class BannerPerformanceDto
    {
        public int BannerId { get; set; }
        public string BannerTitle { get; set; } = string.Empty;
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }

        // Current period stats
        public int CurrentViews { get; set; }
        public int CurrentClicks { get; set; }
        public decimal CurrentCTR { get; set; }

        // Previous period stats (for comparison)
        public int PreviousViews { get; set; }
        public int PreviousClicks { get; set; }
        public decimal PreviousCTR { get; set; }

        // Growth percentages
        public decimal ViewGrowthPercentage { get; set; }
        public decimal ClickGrowthPercentage { get; set; }
        public decimal CTRGrowthPercentage { get; set; }
    }
}
