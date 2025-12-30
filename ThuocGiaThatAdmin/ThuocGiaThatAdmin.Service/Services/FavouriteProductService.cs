using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture;
using ThuocGiaThat.Infrastucture.Common;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class FavouriteProductService
    {
        private readonly TrueMecContext _context;

        public FavouriteProductService(TrueMecContext context)
        {
            _context = context;
        }

        public async Task AddFavoriteAsync(int customerId, int productVariantId, FavouriteProductType type)
        {
            var exists = await _context.FavouriteProducts
                .AnyAsync(f => f.CustomerId == customerId && f.ProductVariantId == productVariantId && f.Type == type);

            if (!exists)
            {
                var favorite = new FavouriteProduct
                {
                    CustomerId = customerId,
                    ProductVariantId = productVariantId,
                    Type = type
                };
                _context.FavouriteProducts.Add(favorite);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFavoriteAsync(int customerId, int productVariantId, FavouriteProductType type)
        {
            var favorite = await _context.FavouriteProducts
                .FirstOrDefaultAsync(f => f.CustomerId == customerId && f.ProductVariantId == productVariantId && f.Type == type);

            if (favorite != null)
            {
                _context.FavouriteProducts.Remove(favorite);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PagedResult<FavouriteProduct>> GetFavoritesAsync(int customerId, FavouriteProductType? type, int pageNumber, int pageSize)
        {
            var query = _context.FavouriteProducts
                .Where(f => f.CustomerId == customerId);

            if (type.HasValue)
            {
                query = query.Where(f => f.Type == type.Value);
            }

            query = query.Include(f => f.ProductVariant)
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
