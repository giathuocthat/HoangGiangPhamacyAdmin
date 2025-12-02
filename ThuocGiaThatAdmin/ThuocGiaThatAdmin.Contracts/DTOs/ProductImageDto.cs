using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    /// <summary>
    /// DTO for creating product images
    /// </summary>
    public class ProductImageDto
    {
        public int Id { get; set; }
        public bool IsPrimary { get; set; } = false;

        [Required(ErrorMessage = "Image URL is required")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string ImageUrl { get; set; } = string.Empty;

        public string? AltText { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Display order must be non-negative")]
        public int DisplayOrder { get; set; } = 0;
    }
}
