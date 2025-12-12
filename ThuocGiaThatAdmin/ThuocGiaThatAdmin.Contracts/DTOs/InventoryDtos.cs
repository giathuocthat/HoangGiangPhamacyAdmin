using System;
using System.ComponentModel.DataAnnotations;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    // ========== Warehouse DTOs ==========
    
    public class WarehouseDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public WarehouseType Type { get; set; }
        public string? Address { get; set; }
        public string? Ward { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? ManagerName { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    
    public class CreateWarehouseDto
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public WarehouseType Type { get; set; }
        
        [MaxLength(500)]
        public string? Address { get; set; }
        
        public string? Ward { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
        
        [Phone]
        public string? PhoneNumber { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }
        
        public string? ManagerName { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
    }
    
    public class UpdateWarehouseDto : CreateWarehouseDto
    {
    }
    
    // ========== Inventory DTOs ==========
    
    public class InventoryDto
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int QuantityOnHand { get; set; }
        public int QuantityReserved { get; set; }
        public int QuantityAvailable { get; set; }
        public int ReorderLevel { get; set; }
        public int MaxStockLevel { get; set; }
        public int ReorderQuantity { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
    }
    
    public class CreateInventoryDto
    {
        [Required]
        public int ProductVariantId { get; set; }
        
        [Required]
        public int WarehouseId { get; set; }
        
        public int ReorderLevel { get; set; } = 10;
        public int MaxStockLevel { get; set; } = 1000;
        public int ReorderQuantity { get; set; } = 50;
        public string? Location { get; set; }
        public string? Notes { get; set; }
    }
    
    public class UpdateInventoryDto
    {
        public int ReorderLevel { get; set; }
        public int MaxStockLevel { get; set; }
        public int ReorderQuantity { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
    }
    
    // ========== Batch DTOs ==========
    
    public class BatchDto
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public DateTime? ManufactureDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int Quantity { get; set; }
        public int QuantitySold { get; set; }
        public int QuantityRemaining { get; set; }
        public decimal? CostPrice { get; set; }
        public string? Supplier { get; set; }
        public string? PurchaseOrderNumber { get; set; }
        public BatchStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    
    public class CreateBatchDto
    {
        [Required]
        public int InventoryId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string BatchNumber { get; set; } = string.Empty;
        
        public DateTime? ManufactureDate { get; set; }
        
        [Required]
        public DateTime ExpiryDate { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal CostPrice { get; set; }
        
        [MaxLength(255)]
        public string? Supplier { get; set; }
        
        [MaxLength(100)]
        public string? PurchaseOrderNumber { get; set; }
        
        public string? Notes { get; set; }
    }
    
    // ========== Transaction DTOs ==========
    
    public class TransactionDto
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int? BatchId { get; set; }
        public string? BatchNumber { get; set; }
        public TransactionType Type { get; set; }
        public int Quantity { get; set; }
        public int QuantityBefore { get; set; }
        public int QuantityAfter { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalValue { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? ReferenceType { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    
    public class PurchaseInventoryDto
    {
        [Required]
        public int WarehouseId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string BatchNumber { get; set; } = string.Empty;
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        public string? Notes { get; set; }
    }
    
    public class PurchaseInventoryResponseDto
    {
        public InventoryDto Inventory { get; set; } = null!;
        public BatchDto Batch { get; set; } = null!;
        public TransactionDto Transaction { get; set; } = null!;
        public string Message { get; set; } = string.Empty;
    }
    
    // ========== Sale Transaction DTOs ==========
    
    public class SaleInventoryDto
    {
        [Required]
        public int ProductVariantId { get; set; }
        
        [Required]
        public int WarehouseId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
        
        /// <summary>
        /// Optional: Order ID if this sale is from an order
        /// </summary>
        public int? OrderId { get; set; }
        
        /// <summary>
        /// Optional: Specific batch to sell from. If not provided, FEFO will be used
        /// </summary>
        public int? BatchId { get; set; }
        
        public string? Notes { get; set; }
    }
    
    // ========== Return Transaction DTOs ==========
    
    public class ReturnInventoryDto
    {
        [Required]
        public int ProductVariantId { get; set; }
        
        [Required]
        public int WarehouseId { get; set; }
        
        /// <summary>
        /// Batch to return to (original batch customer purchased from)
        /// </summary>
        [Required]
        public int BatchId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
        
        /// <summary>
        /// Optional: Order ID if this return is from an order
        /// </summary>
        public int? OrderId { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
        
        public string? Notes { get; set; }
    }
    
    // ========== Return to Supplier DTOs ==========
    
    public class ReturnToSupplierDto
    {
        [Required]
        public int ProductVariantId { get; set; }
        
        [Required]
        public int WarehouseId { get; set; }
        
        [Required]
        public int BatchId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
        
        [MaxLength(255)]
        public string? SupplierName { get; set; }
        
        public string? Notes { get; set; }
    }
    
    // ========== Transfer Transaction DTOs ==========
    
    public class TransferInventoryDto
    {
        [Required]
        public int ProductVariantId { get; set; }
        
        [Required]
        public int FromWarehouseId { get; set; }
        
        [Required]
        public int ToWarehouseId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
        
        /// <summary>
        /// Optional: Specific batch to transfer. If not provided, FEFO will be used
        /// </summary>
        public int? BatchId { get; set; }
        
        [MaxLength(500)]
        public string? Reason { get; set; }
        
        public string? Notes { get; set; }
    }
    
    public class TransferInventoryResponseDto
    {
        public TransactionDto TransferOutTransaction { get; set; } = null!;
        public TransactionDto TransferInTransaction { get; set; } = null!;
        public InventoryDto SourceInventory { get; set; } = null!;
        public InventoryDto DestinationInventory { get; set; } = null!;
        public string Message { get; set; } = string.Empty;
    }
    
    // ========== Adjustment Transaction DTOs ==========
    
    public class AdjustmentInventoryDto
    {
        [Required]
        public int ProductVariantId { get; set; }
        
        [Required]
        public int WarehouseId { get; set; }
        
        /// <summary>
        /// Actual quantity counted during stocktaking
        /// </summary>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Actual quantity cannot be negative")]
        public int ActualQuantity { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
        
        public string? Notes { get; set; }
    }
    
    // ========== Generic Transaction Response ==========
    
    public class InventoryTransactionResponseDto
    {
        public InventoryDto Inventory { get; set; } = null!;
        public TransactionDto Transaction { get; set; } = null!;
        public string Message { get; set; } = string.Empty;
    }
    
    // ========== Transaction Query DTOs ==========
    
    public class TransactionFilterDto
    {
        public TransactionType? Type { get; set; }
        public int? WarehouseId { get; set; }
        public int? ProductVariantId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
    
    
    // ========== Alert DTOs ==========
    
    public class StockAlertDto
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int? BatchId { get; set; }
        public string? BatchNumber { get; set; }
        public AlertType Type { get; set; }
        public AlertPriority Priority { get; set; }
        public string Message { get; set; } = string.Empty;
        public int CurrentQuantity { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsRead { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string? ResolutionNotes { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    
    public class ResolveAlertDto
    {
        [Required]
        [MaxLength(500)]
        public string ResolutionNotes { get; set; } = string.Empty;
    }
}
