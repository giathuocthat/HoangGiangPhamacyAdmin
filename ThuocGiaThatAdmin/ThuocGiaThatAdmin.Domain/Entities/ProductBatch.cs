using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Represents a product batch for tracking inventory by lot number
    /// </summary>
    public class ProductBatch : AuditableEntity
    {
        // Id inherited from AuditableEntity
        
        /// <summary>
        /// Unique batch identifier (e.g., LOT-2024-12-001)
        /// </summary>
        public string BatchNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// Product variant this batch belongs to
        /// </summary>
        public int ProductVariantId { get; set; }
        
        // Product Information
        /// <summary>
        /// Date the product was manufactured
        /// </summary>
        public DateTime? ManufactureDate { get; set; }
        
        /// <summary>
        /// Date the product expires
        /// </summary>
        public DateTime ExpiryDate { get; set; }
        
        /// <summary>
        /// Cost price for this batch
        /// </summary>
        public decimal CostPrice { get; set; }
        
        // Purchase Information
        /// <summary>
        /// Purchase order number this batch was received with
        /// </summary>
        public string? PurchaseOrderNumber { get; set; }
        
        /// <summary>
        /// Supplier name
        /// </summary>
        public string? Supplier { get; set; }
        
        /// <summary>
        /// Date this batch was first received into inventory
        /// </summary>
        public DateTime ReceivedDate { get; set; }
        
        /// <summary>
        /// User who created this batch record
        /// </summary>
        public int CreatedByUserId { get; set; }
        
        // QR Code
        /// <summary>
        /// Path to generated QR code image
        /// </summary>
        public string? QRCodePath { get; set; }
        
        /// <summary>
        /// Whether this batch is still active
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        // Navigation Properties
        public virtual ProductVariant ProductVariant { get; set; } = null!;
    }
}
