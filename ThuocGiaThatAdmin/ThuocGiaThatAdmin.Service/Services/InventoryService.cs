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
            var inventories = await _inventoryRepository.GetAllAsync();
            var inventory = inventories.FirstOrDefault(i => 
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

            // 5. Create batch
            var batch = new InventoryBatch
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
            return new PurchaseInventoryResponseDto
            {
                Inventory = MapToInventoryDto(inventory, productVariant, warehouse),
                Batch = MapToBatchDto(batch),
                Transaction = MapToTransactionDto(transaction, productVariant, warehouse, batch),
                Message = $"Successfully purchased {dto.Quantity} units of {productVariant.SKU}"
            };
        }

        public async Task<IEnumerable<InventoryDto>> GetInventoryByWarehouseAsync(int warehouseId)
        {
            var inventories = await _inventoryRepository.GetAllAsync();
            var filtered = inventories.Where(i => i.WarehouseId == warehouseId).ToList();

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
            var inventories = await _inventoryRepository.GetAllAsync();
            var lowStock = inventories.Where(i => i.QuantityOnHand <= i.ReorderLevel).ToList();

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
