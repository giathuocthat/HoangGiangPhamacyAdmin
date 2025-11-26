using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public class InventoryTransactionRepository : Repository<InventoryTransaction>, IInventoryTransactionRepository
    {
        public InventoryTransactionRepository(TrueMecContext context) : base(context)
        {
        }

        public async Task<IEnumerable<InventoryTransaction>> GetByProductVariantIdAsync(int productVariantId)
        {
            return await _dbSet
                .Include(t => t.Warehouse)
                .Include(t => t.Batch)
                .Where(t => t.ProductVariantId == productVariantId)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryTransaction>> GetByWarehouseIdAsync(int warehouseId)
        {
            return await _dbSet
                .Include(t => t.ProductVariant)
                .Include(t => t.Batch)
                .Where(t => t.WarehouseId == warehouseId)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryTransaction>> GetByTypeAsync(TransactionType type)
        {
            return await _dbSet
                .Include(t => t.ProductVariant)
                .Include(t => t.Warehouse)
                .Where(t => t.Type == type)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryTransaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(t => t.ProductVariant)
                .Include(t => t.Warehouse)
                .Where(t => t.CreatedDate >= startDate && t.CreatedDate <= endDate)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }
    }
}
