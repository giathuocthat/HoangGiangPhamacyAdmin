using System;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO cho CTA response đầy đủ
    /// </summary>
    public class CTAResponseDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string ButtonText { get; set; } = string.Empty;
        public CTAType Type { get; set; }
        public CTAPosition Position { get; set; }
        public CTAStyle Style { get; set; }
        public string? TargetUrl { get; set; }
        public bool OpenInNewTab { get; set; }
        public string? Icon { get; set; }
        public string? ImageUrl { get; set; }
        public string? MobileImageUrl { get; set; }
        public string? BackgroundColor { get; set; }
        public string? TextColor { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsActive { get; set; }
        public int ViewCount { get; set; }
        public int ClickCount { get; set; }
        public string? Metadata { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    /// <summary>
    /// DTO cho tạo CTA mới
    /// </summary>
    public class CreateCTADto
    {
        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string ButtonText { get; set; } = string.Empty;
        public CTAType Type { get; set; }
        public CTAPosition Position { get; set; }
        public CTAStyle Style { get; set; }
        public string? TargetUrl { get; set; }
        public bool OpenInNewTab { get; set; }
        public string? Icon { get; set; }
        public string? ImageUrl { get; set; }
        public string? MobileImageUrl { get; set; }
        public string? BackgroundColor { get; set; }
        public string? TextColor { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Metadata { get; set; }
    }

    /// <summary>
    /// DTO cho cập nhật CTA
    /// </summary>
    public class UpdateCTADto
    {
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string ButtonText { get; set; } = string.Empty;
        public CTAType Type { get; set; }
        public CTAPosition Position { get; set; }
        public CTAStyle Style { get; set; }
        public string? TargetUrl { get; set; }
        public bool OpenInNewTab { get; set; }
        public string? Icon { get; set; }
        public string? ImageUrl { get; set; }
        public string? MobileImageUrl { get; set; }
        public string? BackgroundColor { get; set; }
        public string? TextColor { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsActive { get; set; }
        public string? Metadata { get; set; }
    }

    /// <summary>
    /// DTO cho danh sách CTA (minimal data)
    /// </summary>
    public class CTAListItemDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ButtonText { get; set; } = string.Empty;
        public CTAType Type { get; set; }
        public CTAPosition Position { get; set; }
        public CTAStyle Style { get; set; }
        public bool IsActive { get; set; }
        public int ViewCount { get; set; }
        public int ClickCount { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
