using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class ProductImage : AuditableEntity
    {
        // Id inherited
        public int ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public int DisplayOrder { get; set; } = 0;
        // CreatedDate inherited

        public Product Product { get; set; } = null!;
    }
}
