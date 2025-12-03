using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class ProductVariant : AuditableEntity
    {
        // Id inherited
        public int ProductId { get; set; }
        
        public string SKU { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        
        // Price & Stock
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int StockQuantity { get; set; } = 0;
        public int? MaxSalesQuantity { get; set; }
        
        // Variant specific attributes
        public decimal? Weight { get; set; }
        public string? Dimensions { get; set; }
        public string? ImageUrl { get; set; }
        
        public bool IsActive { get; set; } = true;
        // Dates inherited

        public Product Product { get; set; } = null!;
        public ICollection<VariantOptionValue> VariantOptionValues { get; set; } = new List<VariantOptionValue>();
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}
