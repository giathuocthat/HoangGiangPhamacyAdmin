using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class PriceHistory : AuditableEntity
    {
        // Id inherited
        public int ProductVariantId { get; set; }
        public decimal Price { get; set; }
        // CreatedDate inherited (replaces ChangedAt)
        public DateTime? EndDate { get; set; }
        public string? Reason { get; set; }

        public virtual ProductVariant ProductVariant { get; set; } = null!;
    }
}
