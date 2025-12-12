using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO cho combo response đầy đủ
    /// </summary>
    public class ComboResponseDto
    {
        public int Id { get; set; }
        public int? BannerId { get; set; }
        public string? BannerTitle { get; set; }
        public string ComboCode { get; set; } = string.Empty;
        public string ComboName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal ComboPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public int? DiscountPercentage { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Related data
        public List<ComboItemDto> Items { get; set; } = new();
    }

    /// <summary>
    /// DTO cho tạo combo mới
    /// </summary>
    public class CreateComboDto
    {
        public int? BannerId { get; set; }
        public string ComboCode { get; set; } = string.Empty;
        public string ComboName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal ComboPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public int? DiscountPercentage { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; }

        public List<CreateComboItemDto> Items { get; set; } = new();
    }

    /// <summary>
    /// DTO cho cập nhật combo
    /// </summary>
    public class UpdateComboDto
    {
        public string ComboName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal ComboPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public int? DiscountPercentage { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }

        public List<CreateComboItemDto> Items { get; set; } = new();
    }

    /// <summary>
    /// DTO cho combo item
    /// </summary>
    public class ComboItemDto
    {
        public int Id { get; set; }
        public int ComboId { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string VariantName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? BadgeText { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsRequired { get; set; }
    }

    /// <summary>
    /// DTO cho tạo combo item
    /// </summary>
    public class CreateComboItemDto
    {
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? BadgeText { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsRequired { get; set; } = true;
    }

    /// <summary>
    /// DTO cho danh sách combo (minimal data)
    /// </summary>
    public class ComboListItemDto
    {
        public int Id { get; set; }
        public string ComboCode { get; set; } = string.Empty;
        public string ComboName { get; set; } = string.Empty;
        public decimal OriginalPrice { get; set; }
        public decimal ComboPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int ItemCount { get; set; }
    }
}
