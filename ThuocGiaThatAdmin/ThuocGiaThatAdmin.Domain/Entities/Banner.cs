using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class Banner : AuditableEntity
    {
        public int CampaignId { get; set; }

        public string? BannerCode { get; set; }

        public string? Title { get; set; }

        public string? Subtitle { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public string? MobileImageUrl { get; set; }

        public string? BackgroundColor { get; set; }

        public int BannerType { get; set; }

        public int LinkType { get; set; }

        public string? LinkUrl { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public bool IsActive { get; set; }

        public int ViewCount { get; set; }

        public int ClickCount { get; set; }

        // Navigation Properties
        public virtual Campaign? Campaign { get; set; }

        public virtual ICollection<BannerSection> BannerSections { get; set; } = new List<BannerSection>();
        public virtual ICollection<Combo> Combos { get; set; } = new List<Combo>();
        public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
        public virtual ICollection<BannerAnalytics> BannerAnalytics { get; set; } = new List<BannerAnalytics>();
    }
}
