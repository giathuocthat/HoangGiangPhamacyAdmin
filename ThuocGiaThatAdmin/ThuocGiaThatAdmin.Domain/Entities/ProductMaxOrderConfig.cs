using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class ProductMaxOrderConfig : AuditableEntity
    {
        public int ProductId { get; set; }
        public int MaxQuantityPerOrder { get; set; }
        public int? MaxQuantityPerDay { get; set; }
        public int? MaxQuantityPerMonth { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Reason { get; set; }
        
        // Navigation
        public Product Product { get; set; } = null!;
    }
}
