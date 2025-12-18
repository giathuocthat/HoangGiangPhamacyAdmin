using System;
using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Contracts.DTOs.ProductBatch
{
    public class CreateBatchDto
    {
        [Required(ErrorMessage = "Batch number is required")]
        [StringLength(50, ErrorMessage = "Batch number cannot exceed 50 characters")]
        public string BatchNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product variant ID is required")]
        public int ProductVariantId { get; set; }

        public DateTime? ManufactureDate { get; set; }

        [Required(ErrorMessage = "Expiry date is required")]
        public DateTime ExpiryDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Cost price must be greater than or equal to 0")]
        public decimal CostPrice { get; set; } = 0;

        public string? PurchaseOrderNumber { get; set; }
        public string? Supplier { get; set; }
    }
}
