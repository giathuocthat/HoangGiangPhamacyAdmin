using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO for creating a new product
    /// </summary>
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Category ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be greater than 0")]
        public int CategoryId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Brand ID must be greater than 0")]
        public int? BrandId { get; set; }

        // Basic Info
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Short description cannot exceed 500 characters")]
        public string? ShortDescription { get; set; }

        public string? FullDescription { get; set; }

        [Required(ErrorMessage = "Slug is required")]
        [StringLength(250, ErrorMessage = "Slug cannot exceed 250 characters")]
        [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$",
            ErrorMessage = "Slug must be lowercase, alphanumeric with hyphens only")]
        public string Slug { get; set; } = string.Empty;

        [Url(ErrorMessage = "Invalid URL format")]
        public string? ThumbnailUrl { get; set; }

        // Pharma Specifics
        public string? Ingredients { get; set; }

        public string? UsageInstructions { get; set; }

        public string? Contraindications { get; set; }

        public string? StorageInstructions { get; set; }

        [StringLength(100, ErrorMessage = "Registration number cannot exceed 100 characters")]
        public string? RegistrationNumber { get; set; }

        [StringLength(200, ErrorMessage = "Specification cannot exceed 200 characters")]
        public string? Specification { get; set; }

        public bool IsPrescriptionDrug { get; set; } = false;

        // Metadata
        public bool IsActive { get; set; } = true;

        public bool IsFeatured { get; set; } = false;

        public bool IsPublished { get; set; } = false;
        public string? DrugEfficacy { get; set; }
        public string? DosageInstructions { get; set; }
        public string? Indication { get; set; }
        public string? Overdose { get; set; }

        // Related entities
        public List<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();

        public List<ProductVariantDto> ProductVariants { get; set; } = new List<ProductVariantDto>();

        public List<ProductActiveIngredientDto> ProductActiveIngredients { get; set; } =
            new List<ProductActiveIngredientDto>();
    }
}
