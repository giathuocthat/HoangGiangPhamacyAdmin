using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThat.Infrastucture.Common;

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
        private readonly IRepository<ProductBatch> _productBatchRepository;

        public InventoryService(
            IRepository<Inventory> inventoryRepository,
            IRepository<ProductVariant> productVariantRepository,
            IWarehouseRepository warehouseRepository,
            IInventoryBatchRepository batchRepository,
            IInventoryTransactionRepository transactionRepository,
            IStockAlertRepository alertRepository,
            IRepository<ProductBatch> productBatchRepository)
        {
            _inventoryRepository = inventoryRepository;
            _productVariantRepository = productVariantRepository;
            _warehouseRepository = warehouseRepository;
            _batchRepository = batchRepository;
            _transactionRepository = transactionRepository;
            _alertRepository = alertRepository;
            _productBatchRepository = productBatchRepository;
        }

        public async Task<PurchaseInventoryResponseDto> PurchaseInventoryAsync(PurchaseInventoryDto dto, string? userId = null)
        {
            // 1. Check if warehouse exists and is active
            var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId);
            if (warehouse == null)
            {
                throw new KeyNotFoundException($"Warehouse with ID {dto.WarehouseId} not found");
            }

            if (!warehouse.IsActive)
            {
                throw new InvalidOperationException($"Warehouse '{warehouse.Name}' is not active");
            }

            // 2. Lookup ProductBatch
            var productBatch = await _productBatchRepository.FirstOrDefaultAsync(b => b.BatchNumber == dto.BatchNumber);
            if (productBatch == null)
            {
                throw new KeyNotFoundException($"Batch number '{dto.BatchNumber}' not found. Please create the batch first.");
            }

            // 3. Get Product Variant
            var productVariant = await _productVariantRepository.GetByIdAsync(productBatch.ProductVariantId);
            if (productVariant == null)
            {
                throw new KeyNotFoundException($"Product variant for batch '{dto.BatchNumber}' not found");
            }

            // 4. Get or create inventory record
            var inventory = await _inventoryRepository.FirstOrDefaultAsync(i => 
                i.ProductVariantId == productBatch.ProductVariantId && 
                i.WarehouseId == dto.WarehouseId);

            if (inventory == null)
            {
                inventory = new Inventory
                {
                    ProductVariantId = productBatch.ProductVariantId,
                    WarehouseId = dto.WarehouseId,
                    QuantityOnHand = 0,
                    QuantityReserved = 0,
                    // Default location logic or leave empty
                };
                await _inventoryRepository.AddAsync(inventory);
                await _inventoryRepository.SaveChangesAsync();
            }

            // 5. Get or create inventory batch (linking to physical location)
            // Note: InventoryBatch tracks stock AT THIS WAREHOUSE, ProductBatch tracks master batch info
            var inventoryBatch = await _batchRepository.FirstOrDefaultAsync(b => 
                b.InventoryId == inventory.Id && 
                b.BatchNumber == dto.BatchNumber);

            bool isNewInventoryBatch = false;
            if (inventoryBatch == null)
            {
                // Create new inventory batch
                inventoryBatch = new InventoryBatch
                {
                    InventoryId = inventory.Id,
                    BatchNumber = dto.BatchNumber,
                    ManufactureDate = productBatch.ManufactureDate,
                    ExpiryDate = productBatch.ExpiryDate,
                    Quantity = dto.Quantity,
                    QuantitySold = 0,
                    CostPrice = productBatch.CostPrice,
                    Supplier = productBatch.Supplier,
                    PurchaseOrderNumber = productBatch.PurchaseOrderNumber,
                    Status = DetermineBatchStatus(productBatch.ExpiryDate),
                    Notes = dto.Notes
                };
                
                await _batchRepository.AddAsync(inventoryBatch);
                isNewInventoryBatch = true;
            }
            else
            {
                // Update existing batch - add more quantity
                inventoryBatch.Quantity += dto.Quantity;
                
                // Update notes if provided
                if (!string.IsNullOrEmpty(dto.Notes))
                {
                    inventoryBatch.Notes = string.IsNullOrEmpty(inventoryBatch.Notes) 
                        ? dto.Notes 
                        : $"{inventoryBatch.Notes}; {dto.Notes}";
                }
                
                // Recalculate status just in case (e.g. if expiry changed in master - unlikely here but safe)
                inventoryBatch.Status = DetermineBatchStatus(productBatch.ExpiryDate);
                
                _batchRepository.Update(inventoryBatch);
            }
            
            await _batchRepository.SaveChangesAsync();

            // 6. Update inventory quantity
            var quantityBefore = inventory.QuantityOnHand;
            inventory.QuantityOnHand += dto.Quantity;
            _inventoryRepository.Update(inventory);

            // 7. Create transaction record
            var transaction = new InventoryTransaction
            {
                ProductVariantId = productBatch.ProductVariantId,
                WarehouseId = dto.WarehouseId,
                BatchId = inventoryBatch.Id,
                Type = TransactionType.Purchase,
                Quantity = dto.Quantity,
                QuantityBefore = quantityBefore,
                QuantityAfter = inventory.QuantityOnHand,
                UnitPrice = productBatch.CostPrice, // Use standard cost from master batch
                TotalValue = productBatch.CostPrice * dto.Quantity,
                ReferenceNumber = productBatch.PurchaseOrderNumber,
                ReferenceType = "PurchaseOrder",
                PerformedByUserId = userId,
                Reason = "Purchase from supplier",
                Notes = dto.Notes
            };

            await _transactionRepository.AddAsync(transaction);
            await _transactionRepository.SaveChangesAsync();

            // 8. Check and create alerts if needed
            await CheckAndCreateAlertsAsync(inventory, inventoryBatch);

            // 9. Return response
            var message = isNewInventoryBatch 
                ? $"Successfully added {dto.Quantity} units of batch '{dto.BatchNumber}' ({productVariant.SKU}) to inventory"
                : $"Successfully updated batch '{dto.BatchNumber}' with {dto.Quantity} more units of {productVariant.SKU}";
                
            return new PurchaseInventoryResponseDto
            {
                Inventory = MapToInventoryDto(inventory, productVariant, warehouse),
                Batch = MapToBatchDto(inventoryBatch),
                Transaction = MapToTransactionDto(transaction, productVariant, warehouse, inventoryBatch),
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

        /// <summary>
        /// Get inventories with pagination and search
        /// </summary>
        public async Task<PagedResult<InventoryDto>> GetInventoriesAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchText = null)
        {
            // Get all inventories with related data
            var query = await _inventoryRepository.GetPagedAsync(
                pageNumber,
                pageSize,
                searchText == null ?
                    null :
                    (x) => x.Warehouse.Name.Contains(searchText) || x.ProductVariant.Product.Name.Contains(searchText) || x.ProductVariant.SKU.Contains(searchText),
                string.Empty,
                includes: [x => x.ProductVariant.Product, x => x.Warehouse]
            );

            var listDto = query.Items.Select(x => new InventoryDto()
            {
                Id = x.Id,
                ProductVariantId = x.ProductVariantId,
                WarehouseId = x.WarehouseId,
                QuantityOnHand = x.QuantityOnHand,
                QuantityReserved = x.QuantityReserved,
                QuantityAvailable = x.QuantityAvailable,
                ProductName = x.ProductVariant.Product.Name,
                SKU = x.ProductVariant.SKU,
                WarehouseName = x.Warehouse.Name
            });

            return PagedResult<InventoryDto>.Create(listDto, query.TotalCount, pageNumber, pageSize);
        }

        /// <summary>
        /// Get all batches of a specific inventory with detailed information
        /// </summary>
        public async Task<IEnumerable<InventoryBatchDetailDto>> GetInventoryBatchesAsync(int inventoryId)
        {
            // 1. Check if inventory exists
            var inventory = await _inventoryRepository.GetByIdAsync(inventoryId);
            if (inventory == null)
            {
                throw new KeyNotFoundException($"Inventory with ID {inventoryId} not found");
            }

            // 2. Get product variant and product info
            var productVariant = await _productVariantRepository.GetByIdAsync(inventory.ProductVariantId);
            if (productVariant == null)
            {
                throw new KeyNotFoundException($"Product variant {inventory.ProductVariantId} not found");
            }

            // 3. Get all batches for this inventory
            var batches = await _batchRepository.FindAsync(b => b.InventoryId == inventoryId);

            // 4. Map to DTOs with product information
            var batchDtos = batches.Select(batch => new InventoryBatchDetailDto
            {
                Id = batch.Id,
                InventoryId = batch.InventoryId,
                BatchNumber = batch.BatchNumber,
                ProductName = productVariant.Product?.Name ?? "Unknown",
                SKU = productVariant.SKU,
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
            })
            .OrderBy(b => b.ExpiryDate) // Sort by expiry date (FEFO)
            .ToList();

            return batchDtos;
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
