using System;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Contracts.Responses
{
    /// <summary>
    /// Response model for GoodsReceiptItem
    /// </summary>
    public class GoodsReceiptItemResponse
    {
        public int Id { get; set; }
        public int GoodsReceiptId { get; set; }
        public string GoodsReceiptNumber { get; set; } = string.Empty;
        public int PurchaseOrderItemId { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string? VariantOptions { get; set; }
        public int OrderedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public int AcceptedQuantity { get; set; }
        public int RejectedQuantity { get; set; }
        public QualityStatus QualityStatus { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? LocationCode { get; set; }
        public string? ShelfNumber { get; set; }
        public string? Notes { get; set; }
        public string? InspectionNotes { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Calculated fields
        public decimal AcceptanceRate { get; set; }
        public decimal RejectionRate { get; set; }
        public bool IsFullyReceived { get; set; }
        public int RemainingQuantity { get; set; }
    }

    /// <summary>
    /// List item response for GoodsReceiptItem
    /// </summary>
    public class GoodsReceiptItemListResponse
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int OrderedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public int AcceptedQuantity { get; set; }
        public int RejectedQuantity { get; set; }
        public QualityStatus QualityStatus { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal AcceptanceRate { get; set; }
    }

    /// <summary>
    /// Summary response for GoodsReceiptItem statistics
    /// </summary>
    public class GoodsReceiptItemSummaryResponse
    {
        public int TotalItems { get; set; }
        public int TotalOrderedQuantity { get; set; }
        public int TotalReceivedQuantity { get; set; }
        public int TotalAcceptedQuantity { get; set; }
        public int TotalRejectedQuantity { get; set; }
        public decimal OverallAcceptanceRate { get; set; }
        public decimal OverallRejectionRate { get; set; }
        public int ItemsWithGoodQuality { get; set; }
        public int ItemsWithDamagedQuality { get; set; }
        public int ItemsWithExpiredQuality { get; set; }
        public int ItemsWithDefectiveQuality { get; set; }
    }
}
