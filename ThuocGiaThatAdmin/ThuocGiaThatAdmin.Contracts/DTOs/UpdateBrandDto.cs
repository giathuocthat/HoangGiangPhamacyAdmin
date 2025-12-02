using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    public class UpdateBrandDto
    {
        [Required(ErrorMessage = "Brand name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Slug is required")]
        public string Slug { get; set; } = string.Empty;

        public string? CountryOfOrigin { get; set; }

        public string? Website { get; set; }

        public string? LogoUrl { get; set; }

        public bool IsActive { get; set; }
    }
}
