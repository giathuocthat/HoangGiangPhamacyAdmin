using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class VariantOptionValue : BaseEntity
    {
        // Id inherited
        public int ProductVariantId { get; set; }
        public int ProductOptionValueId { get; set; }

        public virtual ProductVariant ProductVariant { get; set; } = null!;
        public virtual ProductOptionValue ProductOptionValue { get; set; } = null!;
    }
}
