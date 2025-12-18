using System;

namespace ThuocGiaThatAdmin.Contracts.DTOs.ProductBatch
{
    public class GetProductBatchesRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortField { get; set; } = "CreatedDate";
        public string SortOrder { get; set; } = "desc";
    }

    public class ProductBatchResponseDto
    {
        public int Id { get; set; }
        public string BatchNumber { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; }
        public string VariantSKU { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal CostPrice { get; set; }
        public string QRCodePath { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string Supplier { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
