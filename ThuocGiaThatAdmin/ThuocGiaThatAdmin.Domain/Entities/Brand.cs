using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class Brand : AuditableEntity
    {
        // Id inherited
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? CountryOfOrigin { get; set; }
        public string? Website { get; set; }
        public string? LogoUrl { get; set; }
        public bool IsActive { get; set; } = true;
        // Dates inherited

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
