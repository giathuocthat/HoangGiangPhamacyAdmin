using System;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    public class BusinessInfoDto
    {
        public int BusinessTypeId { get; set; }
        public string BusinessTypeName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public string? BusinessRegistrationNumber { get; set; }
        public DateTime? BusinessRegistrationDate { get; set; }
        public string? LegalRepresentative { get; set; }
        public string? BusinessLicenseUrl { get; set; }
        public string? BusinessAddress { get; set; }
        public string? BusinessPhone { get; set; }
        public string? BusinessEmail { get; set; }
    }
}
