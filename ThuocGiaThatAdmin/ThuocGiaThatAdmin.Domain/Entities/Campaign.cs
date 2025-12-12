using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class Campaign : AuditableEntity
    {
        public string? CampaignCode { get; set; }

        public string? CampaignName { get; set; }

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal? Budget { get; set; }

        public bool IsActive { get; set; }

        // Navigation Properties
        public virtual ICollection<Banner> Banners { get; set; } = new List<Banner>();

    }
}
