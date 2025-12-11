using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThat.Infrastucture.Repositories;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class InventoryService
    {
        private readonly IRepository<Inventory> _inventoryRepository;
        private readonly IRepository<ProductVariant> _productVariantRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IInventoryBatchRepository _batchRepository;
        private readonly IInventoryTransactionRepository _transactionRepository;
        private readonly IStockAlertRepository _alertRepository;

        public InventoryService(
            IRepository<Inventory> inventoryRepository,
            IRepository<ProductVariant> productVariantRepository,
            IWarehouseRepository warehouseRepository,
            IInventoryBatchRepository batchRepository,
            IInventoryTransactionRepository transactionRepository,
            IStockAlertRepository alertRepository)
        {
            _inventoryRepository = inventoryRepository;
            _productVariantRepository = productVariantRepository;
            _warehouseRepository = warehouseRepository;
            _batchRepository = batchRepository;
            _transactionRepository = transactionRepository;
            _alertRepository = alertRepository;
        }

        public async Task<PurchaseInventoryResponseDto> PurchaseInventoryAsync(PurchaseInventoryDto dto, string? userId = null)
        {
            // 1. Validate expiry date
            if (dto.ExpiryDate <= DateTime.Now)
            {
                throw new ArgumentException("Expiry date must be in the future");
            }

            // 2. Check if product variant exists
            var productVariant = await _productVariantRepository.GetByIdAsync(dto.ProductVariantId);
            if (productVariant == null)
            {
                throw new KeyNotFoundException($"Product variant with ID {dto.ProductVariantId} not found");
            }

            // 3. Check if warehouse exists and is active
            var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId);
            if (warehouse == null)
            {
                throw new KeyNotFoundException($"Warehouse with ID {dto.WarehouseId} not found");
            }

            if (!warehouse.IsActive)
            {
                throw new InvalidOperationException($"Warehouse '{warehouse.Name}' is not active");
            }

            // 4. Get or create inventory record
            var inventory = await _inventoryRepository.FirstOrDefaultAsync(i => 
                i.ProductVariantId == dto.ProductVariantId && 
                i.WarehouseId == dto.WarehouseId);

            if (inventory == null)
            {
                inventory = new Inventory
                {
                    ProductVariantId = dto.ProductVariantId,
                    WarehouseId = dto.WarehouseId,
                    QuantityOnHand = 0,
                    QuantityReserved = 0,
                    Location = dto.Location
                };
                await _inventoryRepository.AddAsync(inventory);
                await _inventoryRepository.SaveChangesAsync();
            }

            // 5. Get or create batch (check if batch number already exists for this inventory)
            var batch = await _batchRepository.FirstOrDefaultAsync(b => 
                b.InventoryId == inventory.Id && 
                b.BatchNumber == dto.BatchNumber);

            bool isNewBatch = false;
            if (batch == null)
            {
                // Create new batch
                batch = new InventoryBatch
                {
                    InventoryId = inventory.Id,
                    BatchNumber = dto.BatchNumber,
                    ManufactureDate = dto.ManufactureDate,
                    ExpiryDate = dto.ExpiryDate,
                    Quantity = dto.Quantity,
                    QuantitySold = 0,
                    CostPrice = dto.CostPrice,
                    Supplier = dto.Supplier,
                    PurchaseOrderNumber = dto.PurchaseOrderNumber,
                    Status = DetermineBatchStatus(dto.ExpiryDate),
                    Notes = dto.Notes
                };
                
                await _batchRepository.AddAsync(batch);
                isNewBatch = true;
            }
            else
            {
                // Update existing batch - add more quantity to existing batch
                batch.Quantity += dto.Quantity;
                
                // Optionally update other fields if they differ
                if (dto.ManufactureDate.HasValue && batch.ManufactureDate != dto.ManufactureDate)
                {
                    batch.ManufactureDate = dto.ManufactureDate;
                }
                
                if (batch.ExpiryDate != dto.ExpiryDate)
                {
                    batch.ExpiryDate = dto.ExpiryDate;
                    batch.Status = DetermineBatchStatus(dto.ExpiryDate);
                }
                
                // Update cost price to weighted average
                if (dto.CostPrice.HasValue)
                {
                    var existingValue = (batch.CostPrice ?? 0) * (batch.Quantity - dto.Quantity);
                    var newValue = dto.CostPrice.Value * dto.Quantity;
                    batch.CostPrice = (existingValue + newValue) / batch.Quantity;
                }
                
                if (!string.IsNullOrEmpty(dto.Supplier))
                {
                    batch.Supplier = dto.Supplier;
                }
                
                if (!string.IsNullOrEmpty(dto.Notes))
                {
                    batch.Notes = string.IsNullOrEmpty(batch.Notes) 
                        ? dto.Notes 
                        : $"{batch.Notes}; {dto.Notes}";
                }
                
                _batchRepository.Update(batch);
            }
            
            await _batchRepository.SaveChangesAsync();

            // 6. Update inventory quantity
            var quantityBefore = inventory.QuantityOnHand;
            inventory.QuantityOnHand += dto.Quantity;
            if (!string.IsNullOrEmpty(dto.Location))
            {
                inventory.Location = dto.Location;
            }
            _inventoryRepository.Update(inventory);

            // 7. Create transaction record
            var transaction = new InventoryTransaction
            {
                ProductVariantId = dto.ProductVariantId,
                WarehouseId = dto.WarehouseId,
                BatchId = batch.Id,
                Type = TransactionType.Purchase,
                Quantity = dto.Quantity,
                QuantityBefore = quantityBefore,
                QuantityAfter = inventory.QuantityOnHand,
                UnitPrice = dto.CostPrice,
                TotalValue = dto.CostPrice * dto.Quantity,
                ReferenceNumber = dto.PurchaseOrderNumber,
                ReferenceType = "PurchaseOrder",
                PerformedByUserId = userId,
                Reason = "Purchase from supplier",
                Notes = dto.Notes
            };

            await _transactionRepository.AddAsync(transaction);
            await _transactionRepository.SaveChangesAsync();

            // 8. Check and create alerts if needed
            await CheckAndCreateAlertsAsync(inventory, batch);

            // 9. Return response
            var message = isNewBatch 
                ? $"Successfully created new batch and purchased {dto.Quantity} units of {productVariant.SKU}"
                : $"Successfully updated existing batch '{dto.BatchNumber}' with {dto.Quantity} more units of {productVariant.SKU}";
                
            return new PurchaseInventoryResponseDto
            {
                Inventory = MapToInventoryDto(inventory, productVariant, warehouse),
                Batch = MapToBatchDto(batch),
                Transaction = MapToTransactionDto(transaction, productVariant, warehouse, batch),
                Message = message
            };
        }

        public async Task<IEnumerable<InventoryDto>> GetInventoryByWarehouseAsync(int warehouseId)
        {
            var filtered = await _inventoryRepository.FindAsync(i => i.WarehouseId == warehouseId);

            var result = new List<InventoryDto>();
            foreach (var inv in filtered)
            {
                var variant = await _productVariantRepository.GetByIdAsync(inv.ProductVariantId);
                var warehouse = await _warehouseRepository.GetByIdAsync(inv.WarehouseId);
                if (variant != null && warehouse != null)
                {
                    result.Add(MapToInventoryDto(inv, variant, warehouse));
                }
            }

            return result;
        }

        public async Task<IEnumerable<InventoryDto>> GetLowStockInventoriesAsync()
        {
            var lowStock = await _inventoryRepository.FindAsync(i => i.QuantityOnHand <= i.ReorderLevel);

            var result = new List<InventoryDto>();
            foreach (var inv in lowStock)
            {
                var variant = await _productVariantRepository.GetByIdAsync(inv.ProductVariantId);
                var warehouse = await _warehouseRepository.GetByIdAsync(inv.WarehouseId);
                if (variant != null && warehouse != null)
                {
                    result.Add(MapToInventoryDto(inv, variant, warehouse));
                }
            }

            return result;
        }

        // ========== Sale Transaction ==========
        
        public async Task<InventoryTransactionResponseDto> SaleInventoryAsync(SaleInventoryDto dto, string? userId = null)
        {
            // 1. Validate and get entities
            var (inventory, productVariant, warehouse) = await ValidateInventoryOperation(
                dto.ProductVariantId, dto.WarehouseId);

            // 2. Check if sufficient inventory available
            if (inventory.QuantityOnHand < dto.Quantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient inventory. Available: {inventory.QuantityOnHand}, Requested: {dto.Quantity}");
            }

            // 3. Select batch (FEFO or manual)
            InventoryBatch batch;
            if (dto.BatchId.HasValue)
            {
                batch = await _batchRepository.GetByIdAsync(dto.BatchId.Value) 
                    ?? throw new KeyNotFoundException($"Batch {dto.BatchId} not found");
                
                if (batch.InventoryId != inventory.Id)
                {
                    throw new InvalidOperationException("Batch does not belong to this inventory");
                }
            }
            else
            {
                batch = await SelectBatchForOutbound(inventory.Id, dto.Quantity);
            }

            // 4. Update batch quantities
            batch.QuantitySold += dto.Quantity;
            _batchRepository.Update(batch);

            // 5. Update inventory
            var quantityBefore = inventory.QuantityOnHand;
            inventory.QuantityOnHand -= dto.Quantity;
            inventory.QuantityReserved -= Math.Min(inventory.QuantityReserved, dto.Quantity); // Decrease reserved
            _inventoryRepository.Update(inventory);

            // 6. Create transaction
            var transaction = new InventoryTransaction
            {
                ProductVariantId = dto.ProductVariantId,
                WarehouseId = dto.WarehouseId,
                BatchId = batch.Id,
                Type = TransactionType.Sale,
                Quantity = dto.Quantity,
                QuantityBefore = quantityBefore,
                QuantityAfter = inventory.QuantityOnHand,
                ReferenceNumber = dto.OrderId?.ToString(),
                ReferenceType = dto.OrderId.HasValue ? "Order" : "DirectSale",
                PerformedByUserId = userId,
                Reason = "Sale",
                Notes = dto.Notes
            };

            await _transactionRepository.AddAsync(transaction);
            await _transactionRepository.SaveChangesAsync();

            // 7. Check alerts
            await CheckAndCreateAlertsAsync(inventory, batch);

            return new InventoryTransactionResponseDto
            {
                Inventory = MapToInventoryDto(inventory, productVariant, warehouse),
                Transaction = MapToTransactionDto(transaction, productVariant, warehouse, batch),
                Message = $"Successfully sold {dto.Quantity} units of {productVariant.SKU}"
            };
        }

        // ========== Return Transaction ==========
        
        public async Task<InventoryTransactionResponseDto> ReturnInventoryAsync(ReturnInventoryDto dto, string? userId = null)
        {
            // 1. Validate and get entities
            var (inventory, productVariant, warehouse) = await ValidateInventoryOperation(
                dto.ProductVariantId, dto.WarehouseId);

            // 2. Get batch
            var batch = await _batchRepository.GetByIdAsync(dto.BatchId) 
                ?? throw new KeyNotFoundException($"Batch {dto.BatchId} not found");
            
            if (batch.InventoryId != inventory.Id)
            {
                throw new InvalidOperationException("Batch does not belong to this inventory");
            }

            // 3. Update batch quantities (return to original batch)
            if (batch.QuantitySold < dto.Quantity)
            {
                throw new InvalidOperationException(
                    $"Cannot return more than sold. Sold: {batch.QuantitySold}, Return: {dto.Quantity}");
            }
            
            batch.QuantitySold -= dto.Quantity;
            _batchRepository.Update(batch);

            // 4. Update inventory
            var quantityBefore = inventory.QuantityOnHand;
            inventory.QuantityOnHand += dto.Quantity;
            _inventoryRepository.Update(inventory);

            // 5. Create transaction
            var transaction = new InventoryTransaction
            {
                ProductVariantId = dto.ProductVariantId,
                WarehouseId = dto.WarehouseId,
                BatchId = batch.Id,
                Type = TransactionType.Return,
                Quantity = dto.Quantity,
                QuantityBefore = quantityBefore,
                QuantityAfter = inventory.QuantityOnHand,
                ReferenceNumber = dto.OrderId?.ToString(),
                ReferenceType = dto.OrderId.HasValue ? "Order" : "DirectReturn",
                PerformedByUserId = userId,
                Reason = dto.Reason,
                Notes = dto.Notes
            };

            await _transactionRepository.AddAsync(transaction);
            await _transactionRepository.SaveChangesAsync();

            return new InventoryTransactionResponseDto
            {
                Inventory = MapToInventoryDto(inventory, productVariant, warehouse),
                Transaction = MapToTransactionDto(transaction, productVariant, warehouse, batch),
                Message = $"Successfully returned {dto.Quantity} units of {productVariant.SKU}"
            };
        }

        // ========== Return to Supplier Transaction ==========
        
        public async Task<InventoryTransactionResponseDto> ReturnToSupplierAsync(ReturnToSupplierDto dto, string? userId = null)
        {
            // 1. Validate and get entities
            var (inventory, productVariant, warehouse) = await ValidateInventoryOperation(
                dto.ProductVariantId, dto.WarehouseId);

            // 2. Get batch
            var batch = await _batchRepository.GetByIdAsync(dto.BatchId) 
                ?? throw new KeyNotFoundException($"Batch {dto.BatchId} not found");
            
            if (batch.InventoryId != inventory.Id)
            {
                throw new InvalidOperationException("Batch does not belong to this inventory");
            }

            // 3. Check sufficient quantity in batch
            if (batch.QuantityRemaining < dto.Quantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient quantity in batch. Available: {batch.QuantityRemaining}, Requested: {dto.Quantity}");
            }

            // 4. Update batch - reduce total quantity
            batch.Quantity -= dto.Quantity;
            _batchRepository.Update(batch);

            // 5. Update inventory
            var quantityBefore = inventory.QuantityOnHand;
            inventory.QuantityOnHand -= dto.Quantity;
            _inventoryRepository.Update(inventory);

            // 6. Create transaction
            var transaction = new InventoryTransaction
            {
                ProductVariantId = dto.ProductVariantId,
                WarehouseId = dto.WarehouseId,
                BatchId = batch.Id,
                Type = TransactionType.ReturnToSupplier,
                Quantity = dto.Quantity,
                QuantityBefore = quantityBefore,
                QuantityAfter = inventory.QuantityOnHand,
                ReferenceNumber = dto.SupplierName,
                ReferenceType = "SupplierReturn",
                PerformedByUserId = userId,
                Reason = dto.Reason,
                Notes = dto.Notes
            };

            await _transactionRepository.AddAsync(transaction);
            await _transactionRepository.SaveChangesAsync();

            return new InventoryTransactionResponseDto
            {
                Inventory = MapToInventoryDto(inventory, productVariant, warehouse),
                Transaction = MapToTransactionDto(transaction, productVariant, warehouse, batch),
                Message = $"Successfully returned {dto.Quantity} units to supplier"
            };
        }

        // ========== Transfer Transaction ==========
        
        public async Task<TransferInventoryResponseDto> TransferInventoryAsync(TransferInventoryDto dto, string? userId = null)
        {
            // Generate unique reference for this transfer
            var transferRef = $"TRF-{DateTime.Now:yyyyMMddHHmmss}";

            // 1. Validate source warehouse and inventory
            var (sourceInventory, productVariant, sourceWarehouse) = await ValidateInventoryOperation(
                dto.ProductVariantId, dto.FromWarehouseId);

            // 2. Validate destination warehouse
            var destWarehouse = await _warehouseRepository.GetByIdAsync(dto.ToWarehouseId) 
                ?? throw new KeyNotFoundException($"Destination warehouse {dto.ToWarehouseId} not found");
            
            if (!destWarehouse.IsActive)
            {
                throw new InvalidOperationException($"Destination warehouse '{destWarehouse.Name}' is not active");
            }

            // 3. Check sufficient quantity
            if (sourceInventory.QuantityOnHand < dto.Quantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient inventory in source warehouse. Available: {sourceInventory.QuantityOnHand}");
            }

            // 4. Select batch (FEFO or manual)
            InventoryBatch sourceBatch;
            if (dto.BatchId.HasValue)
            {
                sourceBatch = await _batchRepository.GetByIdAsync(dto.BatchId.Value) 
                    ?? throw new KeyNotFoundException($"Batch {dto.BatchId} not found");
            }
            else
            {
                sourceBatch = await SelectBatchForOutbound(sourceInventory.Id, dto.Quantity);
            }

            // 5. Get or create destination inventory
            var destInventory = await _inventoryRepository.FirstOrDefaultAsync(i => 
                i.ProductVariantId == dto.ProductVariantId && 
                i.WarehouseId == dto.ToWarehouseId);

            if (destInventory == null)
            {
                destInventory = new Inventory
                {
                    ProductVariantId = dto.ProductVariantId,
                    WarehouseId = dto.ToWarehouseId,
                    QuantityOnHand = 0,
                    QuantityReserved = 0
                };
                await _inventoryRepository.AddAsync(destInventory);
                await _inventoryRepository.SaveChangesAsync();
            }

            // 6. Create or update destination batch
            var destBatch = await GetOrCreateTransferBatch(destInventory.Id, sourceBatch, dto.Quantity);

            // 7. Update source batch and inventory
            sourceBatch.Quantity -= dto.Quantity;
            if (sourceBatch.Quantity == 0)
            {
                sourceBatch.Status = BatchStatus.OutOfStock;
            }
            _batchRepository.Update(sourceBatch);

            var sourceQtyBefore = sourceInventory.QuantityOnHand;
            sourceInventory.QuantityOnHand -= dto.Quantity;
            _inventoryRepository.Update(sourceInventory);

            // 8. Update destination inventory
            var destQtyBefore = destInventory.QuantityOnHand;
            destInventory.QuantityOnHand += dto.Quantity;
            _inventoryRepository.Update(destInventory);

            // 9. Create TransferOut transaction
            var transferOutTransaction = new InventoryTransaction
            {
                ProductVariantId = dto.ProductVariantId,
                WarehouseId = dto.FromWarehouseId,
                BatchId = sourceBatch.Id,
                Type = TransactionType.TransferOut,
                Quantity = dto.Quantity,
                QuantityBefore = sourceQtyBefore,
                QuantityAfter = sourceInventory.QuantityOnHand,
                ReferenceNumber = transferRef,
                ReferenceType = "Transfer",
                PerformedByUserId = userId,
                Reason = dto.Reason ?? "Transfer to another warehouse",
                Notes = dto.Notes
            };

            // 10. Create TransferIn transaction
            var transferInTransaction = new InventoryTransaction
            {
                ProductVariantId = dto.ProductVariantId,
                WarehouseId = dto.ToWarehouseId,
                BatchId = destBatch.Id,
                Type = TransactionType.TransferIn,
                Quantity = dto.Quantity,
                QuantityBefore = destQtyBefore,
                QuantityAfter = destInventory.QuantityOnHand,
                ReferenceNumber = transferRef,
                ReferenceType = "Transfer",
                PerformedByUserId = userId,
                Reason = dto.Reason ?? "Transfer from another warehouse",
                Notes = dto.Notes
            };

            await _transactionRepository.AddAsync(transferOutTransaction);
            await _transactionRepository.AddAsync(transferInTransaction);
            await _transactionRepository.SaveChangesAsync();

            // 11. Check alerts
            await CheckAndCreateAlertsAsync(sourceInventory, sourceBatch);

            return new TransferInventoryResponseDto
            {
                TransferOutTransaction = MapToTransactionDto(transferOutTransaction, productVariant, sourceWarehouse, sourceBatch),
                TransferInTransaction = MapToTransactionDto(transferInTransaction, productVariant, destWarehouse, destBatch),
                SourceInventory = MapToInventoryDto(sourceInventory, productVariant, sourceWarehouse),
                DestinationInventory = MapToInventoryDto(destInventory, productVariant, destWarehouse),
                Message = $"Successfully transferred {dto.Quantity} units from {sourceWarehouse.Name} to {destWarehouse.Name}"
            };
        }

        // ========== Adjustment Transaction ==========
        
        public async Task<InventoryTransactionResponseDto> AdjustInventoryAsync(AdjustmentInventoryDto dto, string? userId = null)
        {
            // 1. Validate and get entities
            var (inventory, productVariant, warehouse) = await ValidateInventoryOperation(
                dto.ProductVariantId, dto.WarehouseId);

            // 2. Calculate difference
            var quantityBefore = inventory.QuantityOnHand;
            var difference = dto.ActualQuantity - quantityBefore;

            if (difference == 0)
            {
                throw new InvalidOperationException("No adjustment needed. Actual quantity matches system quantity");
            }

            // 3. Update inventory
            inventory.QuantityOnHand = dto.ActualQuantity;
            _inventoryRepository.Update(inventory);

            // 4. Create transaction
            var transaction = new InventoryTransaction
            {
                ProductVariantId = dto.ProductVariantId,
                WarehouseId = dto.WarehouseId,
                BatchId = null, // Adjustment typically doesn't involve specific batch
                Type = TransactionType.Adjustment,
                Quantity = Math.Abs(difference), // Always positive
                QuantityBefore = quantityBefore,
                QuantityAfter = inventory.QuantityOnHand,
                ReferenceNumber = $"ADJ-{DateTime.Now:yyyyMMddHHmmss}",
                ReferenceType = "Adjustment",
                PerformedByUserId = userId,
                Reason = $"{dto.Reason} (Difference: {(difference > 0 ? "+" : "")}{difference})",
                Notes = dto.Notes
            };

            await _transactionRepository.AddAsync(transaction);
            await _transactionRepository.SaveChangesAsync();

            return new InventoryTransactionResponseDto
            {
                Inventory = MapToInventoryDto(inventory, productVariant, warehouse),
                Transaction = MapToTransactionDto(transaction, productVariant, warehouse, null),
                Message = $"Inventory adjusted by {(difference > 0 ? "+" : "")}{difference} units"
            };
        }

        // ========== Helper Methods ==========

        private async Task<(Inventory inventory, ProductVariant variant, Warehouse warehouse)> ValidateInventoryOperation(
            int productVariantId, int warehouseId)
        {
            var productVariant = await _productVariantRepository.GetByIdAsync(productVariantId) 
                ?? throw new KeyNotFoundException($"Product variant {productVariantId} not found");

            var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId) 
                ?? throw new KeyNotFoundException($"Warehouse {warehouseId} not found");

            if (!warehouse.IsActive)
            {
                throw new InvalidOperationException($"Warehouse '{warehouse.Name}' is not active");
            }

            var inventory = await _inventoryRepository.FirstOrDefaultAsync(i => 
                i.ProductVariantId == productVariantId && 
                i.WarehouseId == warehouseId);

            if (inventory == null)
            {
                throw new KeyNotFoundException(
                    $"Inventory not found for variant {productVariantId} in warehouse {warehouseId}");
            }

            return (inventory, productVariant, warehouse);
        }

        private async Task<InventoryBatch> SelectBatchForOutbound(int inventoryId, int quantity)
        {
            var batches = await _batchRepository.FindAsync(b => 
                b.InventoryId == inventoryId && b.QuantityRemaining >= quantity);
            
            var availableBatch = batches
                .OrderBy(b => b.ExpiryDate) // FEFO: First Expired First Out
                .FirstOrDefault();

            if (availableBatch == null)
            {
                throw new InvalidOperationException(
                    $"No batch with sufficient quantity ({quantity}) found. Consider breaking into multiple shipments.");
            }

            return availableBatch;
        }

        private async Task<InventoryBatch> GetOrCreateTransferBatch(
            int destInventoryId, InventoryBatch sourceBatch, int quantity)
        {
            var existingBatch = await _batchRepository.FirstOrDefaultAsync(b => 
                b.InventoryId == destInventoryId && 
                b.BatchNumber == sourceBatch.BatchNumber);

            if (existingBatch != null)
            {
                // Add to existing batch
                existingBatch.Quantity += quantity;
                _batchRepository.Update(existingBatch);
                return existingBatch;
            }
            else
            {
                // Create new batch in destination
                var newBatch = new InventoryBatch
                {
                    InventoryId = destInventoryId,
                    BatchNumber = sourceBatch.BatchNumber,
                    ManufactureDate = sourceBatch.ManufactureDate,
                    ExpiryDate = sourceBatch.ExpiryDate,
                    Quantity = quantity,
                    QuantitySold = 0,
                    CostPrice = sourceBatch.CostPrice,
                    Supplier = sourceBatch.Supplier,
                    Status = DetermineBatchStatus(sourceBatch.ExpiryDate),
                    Notes = $"Transferred from batch {sourceBatch.Id}"
                };
                
                await _batchRepository.AddAsync(newBatch);
                await _batchRepository.SaveChangesAsync();
                return newBatch;
            }
        }

        private BatchStatus DetermineBatchStatus(DateTime expiryDate)
        {
            var monthsUntilExpiry = (expiryDate - DateTime.Now).TotalDays / 30;

            if (monthsUntilExpiry < 1)
                return BatchStatus.Expired;
            else if (monthsUntilExpiry < 3)
                return BatchStatus.NearExpiry;
            else
                return BatchStatus.Active;
        }

        private async Task CheckAndCreateAlertsAsync(Inventory inventory, InventoryBatch batch)
        {
            var monthsUntilExpiry = (batch.ExpiryDate - DateTime.Now).TotalDays / 30;

            if (monthsUntilExpiry < 3 && monthsUntilExpiry > 0)
            {
                var alert = new StockAlert
                {
                    ProductVariantId = inventory.ProductVariantId,
                    WarehouseId = inventory.WarehouseId,
                    BatchId = batch.Id,
                    Type = monthsUntilExpiry < 1 ? AlertType.CriticalExpiry : AlertType.NearExpiry,
                    Priority = monthsUntilExpiry < 1 ? AlertPriority.Critical : AlertPriority.High,
                    Message = $"Batch {batch.BatchNumber} will expire on {batch.ExpiryDate:yyyy-MM-dd}",
                    CurrentQuantity = batch.QuantityRemaining,
                    ExpiryDate = batch.ExpiryDate,
                    IsRead = false,
                    IsResolved = false
                };

                await _alertRepository.AddAsync(alert);
                await _alertRepository.SaveChangesAsync();
            }

            // Check low stock
            if (inventory.QuantityOnHand <= inventory.ReorderLevel)
            {
                var alert = new StockAlert
                {
                    ProductVariantId = inventory.ProductVariantId,
                    WarehouseId = inventory.WarehouseId,
                    Type = inventory.QuantityOnHand == 0 ? AlertType.OutOfStock : AlertType.LowStock,
                    Priority = inventory.QuantityOnHand == 0 ? AlertPriority.Critical : AlertPriority.High,
                    Message = inventory.QuantityOnHand == 0 
                        ? "Product is out of stock" 
                        : $"Low stock: {inventory.QuantityOnHand} units (reorder level: {inventory.ReorderLevel})",
                    CurrentQuantity = inventory.QuantityOnHand,
                    IsRead = false,
                    IsResolved = false
                };

                await _alertRepository.AddAsync(alert);
                await _alertRepository.SaveChangesAsync();
            }
        }

        private static InventoryDto MapToInventoryDto(Inventory inventory, ProductVariant variant, Warehouse warehouse)
        {
            return new InventoryDto
            {
                Id = inventory.Id,
                ProductVariantId = inventory.ProductVariantId,
                ProductName = variant.Product?.Name ?? "Unknown",
                SKU = variant.SKU,
                WarehouseId = inventory.WarehouseId,
                WarehouseName = warehouse.Name,
                QuantityOnHand = inventory.QuantityOnHand,
                QuantityReserved = inventory.QuantityReserved,
                QuantityAvailable = inventory.QuantityAvailable,
                ReorderLevel = inventory.ReorderLevel,
                MaxStockLevel = inventory.MaxStockLevel,
                ReorderQuantity = inventory.ReorderQuantity,
                Location = inventory.Location,
                Notes = inventory.Notes
            };
        }

        private static BatchDto MapToBatchDto(InventoryBatch batch)
        {
            return new BatchDto
            {
                Id = batch.Id,
                InventoryId = batch.InventoryId,
                BatchNumber = batch.BatchNumber,
                ManufactureDate = batch.ManufactureDate,
                ExpiryDate = batch.ExpiryDate,
                Quantity = batch.Quantity,
                QuantitySold = batch.QuantitySold,
                QuantityRemaining = batch.QuantityRemaining,
                CostPrice = batch.CostPrice,
                Supplier = batch.Supplier,
                PurchaseOrderNumber = batch.PurchaseOrderNumber,
                Status = batch.Status,
                Notes = batch.Notes,
                CreatedDate = batch.CreatedDate
            };
        }

        private static TransactionDto MapToTransactionDto(
            InventoryTransaction transaction, 
            ProductVariant variant, 
            Warehouse warehouse, 
            InventoryBatch? batch)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                ProductVariantId = transaction.ProductVariantId,
                ProductName = variant.Product?.Name ?? "Unknown",
                SKU = variant.SKU,
                WarehouseId = transaction.WarehouseId,
                WarehouseName = warehouse.Name,
                BatchId = transaction.BatchId,
                BatchNumber = batch?.BatchNumber,
                Type = transaction.Type,
                Quantity = transaction.Quantity,
                QuantityBefore = transaction.QuantityBefore,
                QuantityAfter = transaction.QuantityAfter,
                UnitPrice = transaction.UnitPrice,
                TotalValue = transaction.TotalValue,
                ReferenceNumber = transaction.ReferenceNumber,
                ReferenceType = transaction.ReferenceType,
                Reason = transaction.Reason,
                Notes = transaction.Notes,
                CreatedDate = transaction.CreatedDate
            };
        }
    }
}
