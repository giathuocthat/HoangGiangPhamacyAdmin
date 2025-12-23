using System;
using System.Collections.Generic;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    // ============ PurchaseOrder DTOs ============

    public class PurchaseOrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public int? SupplierContactId { get; set; }
        public string? SupplierContactName { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public PurchaseOrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public int? ApprovedByUserId { get; set; }
        public string? ApprovedByUserName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? Notes { get; set; }
        public string? Terms { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public List<PurchaseOrderItemDto> Items { get; set; } = new List<PurchaseOrderItemDto>();
        public List<GoodsReceiptDto> GoodsReceipts { get; set; } = new List<GoodsReceiptDto>();
    }

    public class CreatePurchaseOrderDto
    {
        public int? SupplierId { get; set; }
        public int? SupplierContactId { get; set; }
        public int? WarehouseId { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? Notes { get; set; }
        public string? Terms { get; set; }

        public List<CreatePurchaseOrderItemDto> Items { get; set; } = new List<CreatePurchaseOrderItemDto>();
    }

    public class UpdatePurchaseOrderDto
    {
        public int? SupplierId { get; set; }
        public int? SupplierContactId { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public string? Notes { get; set; }
        public string? Terms { get; set; }

        public List<UpdatePurchaseOrderItemDto> Items { get; set; } = new List<UpdatePurchaseOrderItemDto>();
    }

    public class PurchaseOrderListItemDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public PurchaseOrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public int TotalItems { get; set; }
        public int ReceivedItems { get; set; }
        public decimal ReceivedPercentage { get; set; }
    }

    public class ApprovePurchaseOrderDto
    {
        public string? Notes { get; set; }
    }

    public class CancelPurchaseOrderDto
    {
        public string Reason { get; set; } = string.Empty;
    }

    // ============ PurchaseOrderItem DTOs ============

    public class PurchaseOrderItemDto
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ProductVariantId { get; set; }
        public int OrderedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxRate { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string? VariantOptions { get; set; }
        public string? Notes { get; set; }

        // Calculated fields
        public int RemainingQuantity { get; set; }
        public decimal ReceivedPercentage { get; set; }
    }

    public class CreatePurchaseOrderItemDto
    {
        public int ProductVariantId { get; set; }
        public int OrderedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxRate { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdatePurchaseOrderItemDto
    {
        public int Id { get; set; }
        public int OrderedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxRate { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? Notes { get; set; }
        public int ProductVariantId { get; set; }
    }

    // ============ PurchaseOrderHistory DTOs ============

    public class PurchaseOrderHistoryDto
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public string? FromStatus { get; set; }
        public string ToStatus { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public int ChangedByUserId { get; set; }
        public string? ChangedByUserName { get; set; }
        public DateTime ChangedDate { get; set; }
        public string? ChangeDetails { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
    }
}
