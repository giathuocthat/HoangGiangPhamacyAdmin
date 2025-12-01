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
    }
}
