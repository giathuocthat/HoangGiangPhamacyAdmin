using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO for product response with full details
    /// </summary>
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int? BrandId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? FullDescription { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string? Ingredients { get; set; }
        public string? UsageInstructions { get; set; }
        public string? Contraindications { get; set; }
        public string? StorageInstructions { get; set; }
        public string? RegistrationNumber { get; set; }
        public bool? IsPrescriptionDrug { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? DrugEfficacy { get; set; }
        public string? DosageInstructions { get; set; }
        
        // Denormalized fields
        public string? BrandName { get; set; }
        public string? CategoryName { get; set; }
        
        // Collections
        public List<ProductImageResponseDto> Images { get; set; } = new();
        public List<ProductVariantResponseDto> ProductVariants { get; set; } = new();
    }

    /// <summary>
    /// DTO for product image in response
    /// </summary>
    public class ProductImageResponseDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// DTO for product variant in response
    /// </summary>
    public class ProductVariantResponseDto
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int StockQuantity { get; set; }
        public int? MaxSalesQuantity { get; set; }
        public decimal? Weight { get; set; }
        public string? Dimensions { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        
        public List<VariantOptionValueResponseDto> OptionValues { get; set; } = new();
    }

    /// <summary>
    /// DTO for variant option value in response
    /// </summary>
    public class VariantOptionValueResponseDto
    {
        public int OptionValueId { get; set; }
        public string OptionValue { get; set; } = string.Empty;
        public int OptionId { get; set; }
        public string OptionName { get; set; } = string.Empty;
    }
}
