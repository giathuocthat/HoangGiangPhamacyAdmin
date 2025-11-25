using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO for creating product variants
    /// </summary>
    public class ProductVariantDto
    {
        [Required(ErrorMessage = "SKU is required")]
        [StringLength(100, ErrorMessage = "SKU cannot exceed 100 characters")]
        public string SKU { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Barcode cannot exceed 100 characters")]
        public string? Barcode { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Original price must be greater than 0")]
        public decimal? OriginalPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be non-negative")]
        public int StockQuantity { get; set; } = 0;

        [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
        public decimal? Weight { get; set; }

        [StringLength(100, ErrorMessage = "Dimensions cannot exceed 100 characters")]
        public string? Dimensions { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
