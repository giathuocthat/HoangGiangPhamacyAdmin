using System;
using System.Collections.Generic;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO cho banner response đầy đủ
    /// </summary>
    public class BannerResponseDto
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public string CampaignName { get; set; } = string.Empty;
        public string BannerCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? MobileImageUrl { get; set; }
        public string? BackgroundColor { get; set; }
        public BannerType BannerType { get; set; }
        public LinkType LinkType { get; set; }
        public string? LinkUrl { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }
        public int ViewCount { get; set; }
        public int ClickCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Related data
        public List<BannerSectionDto> Sections { get; set; } = new();
        public List<ComboListItemDto> Combos { get; set; } = new();
        public List<int> ProductVariantIds { get; set; } = new();
    }

    /// <summary>
    /// DTO cho tạo banner mới
    /// </summary>
    public class CreateBannerDto
    {
        public int CampaignId { get; set; }
        public string BannerCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? MobileImageUrl { get; set; }
        public string? BackgroundColor { get; set; }
        public BannerType BannerType { get; set; }
        public LinkType LinkType { get; set; }
        public string? LinkUrl { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; } = true;

        // Related data
        public List<CreateBannerSectionDto> Sections { get; set; } = new();
        public List<int> ProductVariantIds { get; set; } = new();
    }

    /// <summary>
    /// DTO cho cập nhật banner
    /// </summary>
    public class UpdateBannerDto
    {
        public int CampaignId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? MobileImageUrl { get; set; }
        public string? BackgroundColor { get; set; }
        public LinkType LinkType { get; set; }
        public string? LinkUrl { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }

        public List<CreateBannerSectionDto> Sections { get; set; } = new();
        public List<int> ProductVariantIds { get; set; } = new();
    }

    /// <summary>
    /// DTO cho banner section
    /// </summary>
    public class BannerSectionDto
    {
        public int Id { get; set; }
        public int BannerId { get; set; }
        public string SectionCode { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO cho tạo banner section
    /// </summary>
    public class CreateBannerSectionDto
    {
        public string SectionCode { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO cho banner slider (frontend display)
    /// </summary>
    public class BannerSliderDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? MobileImageUrl { get; set; }
        public string? BackgroundColor { get; set; }
        public BannerType BannerType { get; set; }
        public LinkType LinkType { get; set; }
        public string? LinkUrl { get; set; }
        public int DisplayOrder { get; set; }

        // Simplified related data for slider
        public List<ProductVariantSliderDto> Products { get; set; } = new();
    }

    /// <summary>
    /// DTO cho product variant trong slider
    /// </summary>
    public class ProductVariantSliderDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? BadgeText { get; set; }
        public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// DTO cho danh sách banner (minimal data)
    /// </summary>
    public class BannerListItemDto
    {
        public int Id { get; set; }
        public string BannerCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public BannerType BannerType { get; set; }
        public bool IsActive { get; set; }
        public int ViewCount { get; set; }
        public int ClickCount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
