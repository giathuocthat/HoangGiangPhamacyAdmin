using System;
using System.ComponentModel.DataAnnotations.Schema;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class FavouriteProduct : AuditableEntity
    {
        public int CustomerId { get; set; }
        public int ProductVariantId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("ProductVariantId")]
        public virtual ProductVariant ProductVariant { get; set; }

        public FavouriteProductType Type { get; set; }
    }
}
