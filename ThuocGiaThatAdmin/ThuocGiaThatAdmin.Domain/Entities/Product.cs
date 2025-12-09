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
        public string? DrugEfficacy { get; set; }
        public string? DosageInstructions { get; set; }
        public string? Specification { get; set; } // Quy cách đóng gói (vd: Hộp 30 viên, Chai 100ml)
        public bool? IsPrescriptionDrug { get; set; } = false;

        // Metadata
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;


        // Navigation properties
        public Category Category { get; set; } = null!;
        public Brand? Brand { get; set; }
        public ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();
        public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductCollectionItem> CollectionItems { get; set; } = new List<ProductCollectionItem>();
        public ProductMaxOrderConfig? MaxOrderConfig { get; set; }
    }
}
