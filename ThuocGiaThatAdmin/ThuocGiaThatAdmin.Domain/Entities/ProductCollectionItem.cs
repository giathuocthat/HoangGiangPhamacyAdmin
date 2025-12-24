using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class ProductCollectionItem : BaseEntity
    {
        public int ProductCollectionId { get; set; }
        public int ProductVariantId { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        // Navigation
        public ProductCollection ProductCollection { get; set; } = null!;
        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
