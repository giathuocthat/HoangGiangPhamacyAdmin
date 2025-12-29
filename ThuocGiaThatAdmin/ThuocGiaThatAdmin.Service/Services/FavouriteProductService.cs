using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture;
using ThuocGiaThat.Infrastucture.Common;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class FavouriteProductService
    {
        private readonly TrueMecContext _context;

        public FavouriteProductService(TrueMecContext context)
        {
            _context = context;
        }

        public async Task AddFavoriteAsync(int customerId, int productVariantId)
        {
            var exists = await _context.FavouriteProducts
                .AnyAsync(f => f.CustomerId == customerId && f.ProductVariantId == productVariantId);

            if (!exists)
            {
                var favorite = new FavouriteProduct
                {
                    CustomerId = customerId,
                    ProductVariantId = productVariantId
                };
                _context.FavouriteProducts.Add(favorite);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFavoriteAsync(int customerId, int productVariantId)
        {
            var favorite = await _context.FavouriteProducts
                .FirstOrDefaultAsync(f => f.CustomerId == customerId && f.ProductVariantId == productVariantId);

            if (favorite != null)
            {
                _context.FavouriteProducts.Remove(favorite);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PagedResult<FavouriteProduct>> GetFavoritesAsync(int customerId, int pageNumber, int pageSize)
        {
            var query = _context.FavouriteProducts
                .Where(f => f.CustomerId == customerId)
                .Include(f => f.ProductVariant)
                .OrderByDescending(f => f.CreatedDate);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<FavouriteProduct>(items, totalCount, pageNumber, pageSize);
        }
    }
}
