using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public class ProductCollectionRepository : Repository<ProductCollection>, IProductCollectionRepository
    {
        public ProductCollectionRepository(TrueMecContext context) : base(context)
        {
        }

        public async Task<ProductCollection?> GetBySlugAsync(string slug)
        {
            return await _dbSet
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.Slug == slug);
        }

        public async Task<List<ProductCollection>> GetActiveCollectionsAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(c => c.IsActive)
                .Where(c => c.StartDate == null || c.StartDate <= now)
                .Where(c => c.EndDate == null || c.EndDate >= now)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        public async Task<ProductCollection?> GetWithProductsAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Images)
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.ProductVariants)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddProductToCollectionAsync(int collectionId, int productId, int displayOrder)
        {
            var item = new ProductCollectionItem
            {
                ProductCollectionId = collectionId,
                ProductId = productId,
                DisplayOrder = displayOrder,
                AddedDate = DateTime.UtcNow
            };
            
            await _context.ProductCollectionItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveProductFromCollectionAsync(int collectionId, int productId)
        {
            var item = await _context.ProductCollectionItems
                .FirstOrDefaultAsync(i => i.ProductCollectionId == collectionId && i.ProductId == productId);

            if (item != null)
            {
                _context.ProductCollectionItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ProductCollection> AddAsync(ProductCollection collection)
        {
            await _dbSet.AddAsync(collection);
            await _context.SaveChangesAsync();
            return collection;
        }

        public async Task UpdateAsync(ProductCollection collection)
        {
            _dbSet.Update(collection);
            await _context.SaveChangesAsync();
        }
    }
}
