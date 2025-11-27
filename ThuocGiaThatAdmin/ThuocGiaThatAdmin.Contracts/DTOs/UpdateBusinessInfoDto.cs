using System;
using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    public class UpdateBusinessInfoDto
    {
        [Required(ErrorMessage = "Business type is required")]
        public int BusinessTypeId { get; set; }

        [Required(ErrorMessage = "Company name is required")]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tax code is required")]
        [StringLength(20)]
        [RegularExpression(@"^\d{10,13}$", ErrorMessage = "Tax code must be 10-13 digits")]
        public string TaxCode { get; set; } = string.Empty;

        [StringLength(50)]
        public string? BusinessRegistrationNumber { get; set; }

        public DateTime? BusinessRegistrationDate { get; set; }

        [StringLength(100)]
        public string? LegalRepresentative { get; set; }

        [StringLength(500)]
        public string? BusinessLicenseUrl { get; set; }

        [StringLength(500)]
        public string? BusinessAddress { get; set; }

        [Phone]
        [StringLength(20)]
        public string? BusinessPhone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? BusinessEmail { get; set; }
    }
}
