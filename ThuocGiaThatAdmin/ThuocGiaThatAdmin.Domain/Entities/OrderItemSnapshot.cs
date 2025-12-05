using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// OrderItemSnapshot - Snapshot of product information at the time of order creation
    /// This ensures historical order data remains accurate even if product details change
    /// </summary>
    public class OrderItemSnapshot : BaseEntity
    {
        // Id inherited
        public int OrderItemId { get; set; }
        
        // Product References
        public int ProductId { get; set; }
        public int ProductVariantId { get; set; }
        
        // Basic Product Information
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public string? ShortDescription { get; set; }
        public string? FullDescription { get; set; }
        public string? VariantAttributes { get; set; } // JSON string of variant attributes
        
        // Pricing Information
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        
        // Image Information
        public string? ThumbnailUrl { get; set; }
        public string? VariantImageUrl { get; set; }
        
        // Category & Brand Information
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }
        
        // Pharmaceutical Information (nullable for non-drug products)
        public string? Ingredients { get; set; }
        public string? UsageInstructions { get; set; }
        public string? Contraindications { get; set; }
        public string? StorageInstructions { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? DrugEfficacy { get; set; }
        public string? DosageInstructions { get; set; }
        public bool? IsPrescriptionDrug { get; set; }
        
        // Navigation property
        public OrderItem OrderItem { get; set; } = null!;
    }
}
