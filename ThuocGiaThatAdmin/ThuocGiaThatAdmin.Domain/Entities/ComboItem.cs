using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class ComboItem : AuditableEntity
    {
        public int ComboId { get; set; }

        public int ProductVariantId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public string? BadgeText { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsRequired { get; set; }

        // Navigation Properties
        public virtual Combo? Combo { get; set; }

        public virtual ProductVariant? ProductVariant { get; set; }
    }
}
