using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public class WarehouseRepository : Repository<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(TrueMecContext context) : base(context)
        {
        }

        public async Task<Warehouse?> GetByCodeAsync(string code)
        {
            return await _dbSet.FirstOrDefaultAsync(w => w.Code == code);
        }

        public async Task<IEnumerable<Warehouse>> GetActiveWarehousesAsync()
        {
            return await _dbSet
                .Where(w => w.IsActive)
                .OrderBy(w => w.Name)
                .ToListAsync();
        }

        public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null)
        {
            var query = _dbSet.Where(w => w.Code == code);
            
            if (excludeId.HasValue)
            {
                query = query.Where(w => w.Id != excludeId.Value);
            }
            
            return !await query.AnyAsync();
        }
    }
}
