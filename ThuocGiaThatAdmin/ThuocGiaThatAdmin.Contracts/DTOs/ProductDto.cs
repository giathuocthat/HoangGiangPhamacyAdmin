using System;
using System.Collections.Generic;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int? BrandId { get; set; }
        public string? BrandName { get; set; }

        // Basic Info
        public string Name { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? FullDescription { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }

        // Pharma Specifics
        public string? Ingredients { get; set; }
        public string? UsageInstructions { get; set; }
        public string? Contraindications { get; set; }
        public string? StorageInstructions { get; set; }
        public string? RegistrationNumber { get; set; }
        public bool IsPrescriptionDrug { get; set; }

        // Metadata
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public ProductSourceType SourceType { get; set; }
        public bool IsHGSGSelected { get; set; }
        public DateTime CreatedDate { get; set; }

        // Related entities
        public List<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
        public List<ProductVariantDto> ProductVariants { get; set; } = new List<ProductVariantDto>();
        public ProductMaxOrderConfigDto? MaxOrderConfig { get; set; }
    }

    public class ProductMaxOrderConfigDto
    {
        public int MaxQuantityPerOrder { get; set; }
        public int? MaxQuantityPerDay { get; set; }
        public int? MaxQuantityPerMonth { get; set; }
        public string? Reason { get; set; }
    }
}
