using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public class ProductMaxOrderConfigRepository : Repository<ProductMaxOrderConfig>, IProductMaxOrderConfigRepository
    {
        public ProductMaxOrderConfigRepository(TrueMecContext context) : base(context)
        {
        }

        public async Task<ProductMaxOrderConfig?> GetByProductIdAsync(int productId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.ProductId == productId);
        }

        public async Task<List<ProductMaxOrderConfig>> GetActiveConfigsAsync()
        {
            return await _dbSet
                .Where(c => c.IsActive)
                .Include(c => c.Product)
                .ToListAsync();
        }

        public new async Task<ProductMaxOrderConfig> AddAsync(ProductMaxOrderConfig config)
        {
            await _dbSet.AddAsync(config);
            await _context.SaveChangesAsync();
            return config;
        }

        public async Task UpdateAsync(ProductMaxOrderConfig config)
        {
            _dbSet.Update(config);
            await _context.SaveChangesAsync();
        }
    }
}
