using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO for creating product variants
    /// </summary>
    public class ProductVariantDto
    {
        public int Id { get; set; }
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

        [Range(1, int.MaxValue, ErrorMessage = "Max sales quantity must be greater than 0")]
        public int? MaxSalesQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
        public decimal? Weight { get; set; }

        [StringLength(100, ErrorMessage = "Dimensions cannot exceed 100 characters")]
        public string? Dimensions { get; set; }

        
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public List<int> VariantOptionValueIds { get; set; } = new();
    }
}
