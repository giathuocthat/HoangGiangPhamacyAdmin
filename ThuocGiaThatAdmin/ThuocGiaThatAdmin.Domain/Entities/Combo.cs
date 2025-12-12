using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class Combo : AuditableEntity
    {
        public int? BannerId { get; set; }

        public string? ComboCode { get; set; }

        public string? ComboName { get; set; }

        public string? Description { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal ComboPrice { get; set; }

        public decimal DiscountAmount { get; set; }

        public int? DiscountPercentage { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public bool IsActive { get; set; }

        public int DisplayOrder { get; set; }

        public virtual Banner? Banner { get; set; }

        public virtual ICollection<ComboItem> ComboItems { get; set; } = new List<ComboItem>();
    }
}
