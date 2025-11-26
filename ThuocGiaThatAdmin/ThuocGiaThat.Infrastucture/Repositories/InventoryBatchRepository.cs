using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public class InventoryBatchRepository : Repository<InventoryBatch>, IInventoryBatchRepository
    {
        public InventoryBatchRepository(TrueMecContext context) : base(context)
        {
        }

        public async Task<IEnumerable<InventoryBatch>> GetByInventoryIdAsync(int inventoryId)
        {
            return await _dbSet
                .Where(b => b.InventoryId == inventoryId)
                .OrderBy(b => b.ExpiryDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryBatch>> GetNearExpiryBatchesAsync(int months = 3)
        {
            var thresholdDate = DateTime.Now.AddMonths(months);
            
            return await _dbSet
                .Include(b => b.Inventory)
                    .ThenInclude(i => i.ProductVariant)
                .Include(b => b.Inventory)
                    .ThenInclude(i => i.Warehouse)
                .Where(b => b.ExpiryDate <= thresholdDate 
                         && b.ExpiryDate > DateTime.Now
                         && b.QuantityRemaining > 0
                         && b.Status != BatchStatus.Expired)
                .OrderBy(b => b.ExpiryDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryBatch>> GetExpiredBatchesAsync()
        {
            return await _dbSet
                .Include(b => b.Inventory)
                    .ThenInclude(i => i.ProductVariant)
                .Include(b => b.Inventory)
                    .ThenInclude(i => i.Warehouse)
                .Where(b => b.ExpiryDate <= DateTime.Now && b.QuantityRemaining > 0)
                .OrderBy(b => b.ExpiryDate)
                .ToListAsync();
        }

        public async Task<InventoryBatch?> GetOldestActiveBatchAsync(int inventoryId)
        {
            return await _dbSet
                .Where(b => b.InventoryId == inventoryId 
                         && b.Status == BatchStatus.Active 
                         && b.QuantityRemaining > 0)
                .OrderBy(b => b.ExpiryDate)
                .FirstOrDefaultAsync();
        }
    }
}
