using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Product entity - Sản phẩm (Thuốc, vitamin, chất bổ sung)
    /// </summary>
    public class Product : AuditableEntity
    {
        // Id inherited
        public int CategoryId { get; set; }
        public int? BrandId { get; set; }

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
        public bool IsPrescriptionDrug { get; set; } = false;

        // Metadata
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;
        // Dates inherited

        // Navigation properties
        public virtual Category Category { get; set; } = null!;
        public virtual Brand? Brand { get; set; }
        public virtual ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();
        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    }
}
