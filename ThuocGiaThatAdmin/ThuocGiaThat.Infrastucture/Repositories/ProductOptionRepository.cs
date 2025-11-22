using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Data;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository implementation for ProductOption entity
    /// </summary>
    public class ProductOptionRepository : Repository<ProductOption>, IProductOptionRepository
    {
        public ProductOptionRepository(AppContext context) : base(context)
        {
        }

        /// <summary>
        /// Get product options with pagination, including their values
        /// </summary>
        public async Task<(IEnumerable<ProductOption> ProductOptions, int TotalCount)> GetProductOptionsWithPagingAsync(int pageNumber, int pageSize)
        {
            var query = _context.Set<ProductOption>()
                .Include(po => po.ProductOptionValues.OrderBy(v => v.DisplayOrder))
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var productOptions = await query
                .OrderBy(po => po.ProductId)
                .ThenBy(po => po.DisplayOrder)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (productOptions, totalCount);
        }

        /// <summary>
        /// Get product option by ID with its values
        /// </summary>
        public async Task<ProductOption?> GetByIdWithValuesAsync(int id)
        {
            return await _context.Set<ProductOption>()
                .Include(po => po.ProductOptionValues.OrderBy(v => v.DisplayOrder))
                .FirstOrDefaultAsync(po => po.Id == id);
        }

        /// <summary>
        /// Check if product option is being used by any product
        /// </summary>
        public async Task<bool> IsUsedByProductAsync(int productOptionId)
        {
            // Check if the ProductOption exists and has a ProductId reference
            return await _context.Set<ProductOption>()
                .AnyAsync(po => po.Id == productOptionId && po.ProductId > 0);
        }
    }
}
