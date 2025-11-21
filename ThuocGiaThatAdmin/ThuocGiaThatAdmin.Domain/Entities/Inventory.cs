using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class Inventory : AuditableEntity
    {
        // Id inherited
        public int ProductVariantId { get; set; }
        public int? WarehouseId { get; set; }
        public int Quantity { get; set; } = 0;
        // UpdatedDate inherited (replaces LastUpdated)

        public virtual ProductVariant ProductVariant { get; set; } = null!;
    }
}
