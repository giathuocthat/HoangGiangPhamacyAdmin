using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Data;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository implementation for Brand entity
    /// </summary>
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        public BrandRepository(AppContext context) : base(context)
        {
        }

        /// <summary>
        /// Get brands with pagination
        /// </summary>
        public async Task<(IEnumerable<Brand> Brands, int TotalCount)> GetBrandsWithPagingAsync(int pageNumber, int pageSize)
        {
            var query = _context.Set<Brand>().AsQueryable();

            var totalCount = await query.CountAsync();

            var brands = await query
                .OrderBy(b => b.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (brands, totalCount);
        }

        /// <summary>
        /// Get brand by slug
        /// </summary>
        public async Task<Brand?> GetBySlugAsync(string slug)
        {
            return await _context.Set<Brand>()
                .FirstOrDefaultAsync(b => b.Slug == slug);
        }

        /// <summary>
        /// Check if brand has any products
        /// </summary>
        public async Task<bool> HasProductsAsync(int brandId)
        {
            return await _context.Set<Product>()
                .AnyAsync(p => p.BrandId == brandId);
        }
    }
}
