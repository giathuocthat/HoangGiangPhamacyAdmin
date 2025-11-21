using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        // Id inherited
        public int OrderId { get; set; }
        public int ProductVariantId { get; set; }
        public int ProductId { get; set; }
        
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalLineAmount { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual ProductVariant ProductVariant { get; set; } = null!;
    }
}
