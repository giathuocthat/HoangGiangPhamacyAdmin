using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository implementation cho warehouse picking operations
    /// </summary>
    public class WarehousePickingRepository : IWarehousePickingRepository
    {
        private readonly TrueMecContext _context;

        public WarehousePickingRepository(TrueMecContext context)
        {
            _context = context;
        }

        public async Task<BatchLocationStock?> GetBatchLocationAsync(
            string locationCode, 
            string batchNumber, 
            int warehouseId)
        {
            return await _context.BatchLocationStocks
                .Include(b => b.InventoryBatch)
                .Include(b => b.ProductVariant)
                    .ThenInclude(v => v.Product)
                .FirstOrDefaultAsync(b => 
                    b.LocationCode == locationCode &&
                    b.InventoryBatch.BatchNumber == batchNumber &&
                    b.WarehouseId == warehouseId);
        }

        public async Task AddMovementAsync(LocationStockMovement movement)
        {
            await _context.LocationStockMovements.AddAsync(movement);
        }

        public async Task UpdateBatchLocationStockAsync(BatchLocationStock stock)
        {
            _context.BatchLocationStocks.Update(stock);
            await Task.CompletedTask;
        }

        public async Task<BatchLocationStock> GetOrCreateDestinationStockAsync(
            int inventoryBatchId,
            int productVariantId,
            int warehouseId,
            string destinationLocationCode)
        {
            // Tìm existing stock tại destination
            var existingStock = await _context.BatchLocationStocks
                .FirstOrDefaultAsync(b =>
                    b.InventoryBatchId == inventoryBatchId &&
                    b.WarehouseId == warehouseId &&
                    b.LocationCode == destinationLocationCode);

            if (existingStock != null)
            {
                return existingStock;
            }

            // Tạo mới nếu chưa tồn tại
            var newStock = new BatchLocationStock
            {
                InventoryBatchId = inventoryBatchId,
                ProductVariantId = productVariantId,
                WarehouseId = warehouseId,
                LocationCode = destinationLocationCode,
                Quantity = 0,
                QuantityReserved = 0,
                IsPrimaryLocation = false,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            await _context.BatchLocationStocks.AddAsync(newStock);
            return newStock;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
