using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public class ProductVariantRepository : Repository<ProductVariant>, IProductVariantRepository
    {
        public ProductVariantRepository(TrueMecContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductVariant>> GetByIdsAsync(IList<int> ids)
        {
            return await _context.ProductVariants.Where(x => ids.Contains(x.Id))
                .Include(x => x.Product.Brand)
                .ToListAsync();
        }

        public async Task<ProductVariant?> GetVariantWithProduct(int variantId)
        {
            return await _context.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.Id == variantId);
        }

        public async Task<ProductVariant?> GetDetail(int variantId)
        {
            return await _context.ProductVariants
                .Include(v => v.Product)
                    .ThenInclude(p => p.Brand)
                .Include(v => v.Product)
                    .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(v => v.Id == variantId);
        }
    }
}
