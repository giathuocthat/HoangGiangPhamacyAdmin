using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO cho campaign response
    /// </summary>
    public class CampaignResponseDto
    {
        public int Id { get; set; }
        public string CampaignCode { get; set; } = string.Empty;
        public string CampaignName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? Budget { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Related data
        public List<BannerListItemDto> Banners { get; set; } = new();
    }

    /// <summary>
    /// DTO cho tạo campaign mới
    /// </summary>
    public class CreateCampaignDto
    {
        public string CampaignCode { get; set; } = string.Empty;
        public string CampaignName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? Budget { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO cho cập nhật campaign
    /// </summary>
    public class UpdateCampaignDto
    {
        public string CampaignName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? Budget { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO cho danh sách campaign (minimal data)
    /// </summary>
    public class CampaignListItemDto
    {
        public int Id { get; set; }
        public string CampaignCode { get; set; } = string.Empty;
        public string CampaignName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public int BannerCount { get; set; }
    }
}
