using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class ProductOptionValue : BaseEntity
    {
        // Id inherited
        public int ProductOptionId { get; set; }
        public string Value { get; set; } = string.Empty; // e.g., "Red", "XL"
        public int DisplayOrder { get; set; } = 0;

        public ProductOption ProductOption { get; set; } = null!;

        public List<VariantOptionValue> VariantOptionValues { get; set; } = new List<VariantOptionValue>();
    }
}
