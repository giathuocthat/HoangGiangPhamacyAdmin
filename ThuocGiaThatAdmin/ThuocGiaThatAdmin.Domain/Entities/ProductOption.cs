using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class ProductOption : BaseEntity
    {
        // Id inherited
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty; // e.g., "Color", "Size"
        public int DisplayOrder { get; set; } = 0;

        public virtual Product Product { get; set; } = null!;
    }
}
