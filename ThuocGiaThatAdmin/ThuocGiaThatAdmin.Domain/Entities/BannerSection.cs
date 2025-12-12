using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class BannerSection : AuditableEntity
    {
        public int BannerId { get; set; }

        public string? SectionCode { get; set; }

        public string? SectionName { get; set; }

        public string? Content { get; set; }

        public string? ImageUrl { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        // Navigation Properties
        public virtual Banner? Banner { get; set; }
    }
}
