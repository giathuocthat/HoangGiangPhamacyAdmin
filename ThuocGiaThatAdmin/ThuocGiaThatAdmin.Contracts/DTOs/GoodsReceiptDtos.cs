using System;
using System.Collections.Generic;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    // ============ GoodsReceipt DTOs ============

    public class GoodsReceiptDto
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public int PurchaseOrderId { get; set; }
        public string PurchaseOrderNumber { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public GoodsReceiptStatus Status { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int? ReceivedByUserId { get; set; }
        public string? ReceivedByUserName { get; set; }
        public int? InspectedByUserId { get; set; }
        public string? InspectedByUserName { get; set; }
        public string? ShippingCarrier { get; set; }
        public string? TrackingNumber { get; set; }
        public string? VehicleNumber { get; set; }
        public string? Notes { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public List<GoodsReceiptItemDto> Items { get; set; } = new List<GoodsReceiptItemDto>();
    }

    public class CreateGoodsReceiptDto
    {
        public int PurchaseOrderId { get; set; }
        public int WarehouseId { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public string? ShippingCarrier { get; set; }
        public string? TrackingNumber { get; set; }
        public string? VehicleNumber { get; set; }
        public string? Notes { get; set; }

        public List<CreateGoodsReceiptItemDto> Items { get; set; } = new List<CreateGoodsReceiptItemDto>();
    }

    public class UpdateGoodsReceiptDto
    {
        public DateTime? ScheduledDate { get; set; }
        public string? ShippingCarrier { get; set; }
        public string? TrackingNumber { get; set; }
        public string? VehicleNumber { get; set; }
        public string? Notes { get; set; }

        public List<UpdateGoodsReceiptItemDto> Items { get; set; } = new List<UpdateGoodsReceiptItemDto>();
    }

    public class GoodsReceiptListItemDto
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public string PurchaseOrderNumber { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public GoodsReceiptStatus Status { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public int TotalItems { get; set; }
        public int AcceptedItems { get; set; }
        public int RejectedItems { get; set; }
    }

    public class ReceiveGoodsDto
    {
        public int ReceivedByUserId { get; set; }
        public int? InspectedByUserId { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string? Notes { get; set; }

        public List<ReceiveGoodsItemDto> Items { get; set; } = new List<ReceiveGoodsItemDto>();
    }

    public class ReceiveGoodsItemDto
    {
        public int GoodsReceiptItemId { get; set; }
        public int ReceivedQuantity { get; set; }
        public int AcceptedQuantity { get; set; }
        public int RejectedQuantity { get; set; }
        public QualityStatus QualityStatus { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? LocationCode { get; set; }
        public string? ShelfNumber { get; set; }
        public string? InspectionNotes { get; set; }
    }

    public class CompleteGoodsReceiptDto
    {
        public string? Notes { get; set; }
    }

    public class RejectGoodsReceiptDto
    {
        public string RejectionReason { get; set; } = string.Empty;
    }

    // ============ GoodsReceiptItem DTOs ============

    public class GoodsReceiptItemDto
    {
        public int Id { get; set; }
        public int GoodsReceiptId { get; set; }
        public int PurchaseOrderItemId { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int OrderedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public int AcceptedQuantity { get; set; }
        public QualityStatus QualityStatus { get; set; }
        public int RejectedQuantity { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? LocationCode { get; set; }
        public string? ShelfNumber { get; set; }
        public string? Notes { get; set; }
        public string? InspectionNotes { get; set; }
    }

    public class CreateGoodsReceiptItemDto
    {
        public int PurchaseOrderItemId { get; set; }
        public int OrderedQuantity { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateGoodsReceiptItemDto
    {
        public int Id { get; set; }
        public int OrderedQuantity { get; set; }
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO for updating quality inspection
    /// </summary>
    public class UpdateQualityInspectionDto
    {
        public int ReceivedQuantity { get; set; }
        public int AcceptedQuantity { get; set; }
        public int RejectedQuantity { get; set; }
        public QualityStatus QualityStatus { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? InspectionNotes { get; set; }
    }

    /// <summary>
    /// DTO for updating item location in warehouse
    /// </summary>
    public class UpdateItemLocationDto
    {
        public string? LocationCode { get; set; }
        public string? ShelfNumber { get; set; }
        public string? Notes { get; set; }
    }
}
