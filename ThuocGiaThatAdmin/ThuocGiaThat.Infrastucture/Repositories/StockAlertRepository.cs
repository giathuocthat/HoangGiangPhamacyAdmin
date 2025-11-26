using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public class StockAlertRepository : Repository<StockAlert>, IStockAlertRepository
    {
        public StockAlertRepository(TrueMecContext context) : base(context)
        {
        }

        public async Task<IEnumerable<StockAlert>> GetUnreadAlertsAsync()
        {
            return await _dbSet
                .Include(a => a.ProductVariant)
                .Include(a => a.Warehouse)
                .Include(a => a.Batch)
                .Where(a => !a.IsRead)
                .OrderByDescending(a => a.Priority)
                .ThenByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockAlert>> GetUnresolvedAlertsAsync()
        {
            return await _dbSet
                .Include(a => a.ProductVariant)
                .Include(a => a.Warehouse)
                .Include(a => a.Batch)
                .Where(a => !a.IsResolved)
                .OrderByDescending(a => a.Priority)
                .ThenByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockAlert>> GetByTypeAsync(AlertType type)
        {
            return await _dbSet
                .Include(a => a.ProductVariant)
                .Include(a => a.Warehouse)
                .Where(a => a.Type == type)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockAlert>> GetByPriorityAsync(AlertPriority priority)
        {
            return await _dbSet
                .Include(a => a.ProductVariant)
                .Include(a => a.Warehouse)
                .Where(a => a.Priority == priority)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }
    }
}
