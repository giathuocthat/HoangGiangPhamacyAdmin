using System;

namespace ThuocGiaThatAdmin.Contracts.DTOs.ProductBatch
{
    public class BatchLookupResponseDto
    {
        public int Id { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string VariantSKU { get; set; } = string.Empty;
        public DateTime? ManufactureDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal CostPrice { get; set; }
        public string? QRCodePath { get; set; }
        public string? PurchaseOrderNumber { get; set; }
        public string? Supplier { get; set; }
        public bool IsActive { get; set; }
    }
}
