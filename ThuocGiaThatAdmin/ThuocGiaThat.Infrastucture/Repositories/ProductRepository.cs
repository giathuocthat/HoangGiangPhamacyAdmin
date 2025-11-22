using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository implementation for Product entity
    /// </summary>
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppContext context) : base(context)
        {
        }

        /// <summary>
        /// Get all products by category ID
        /// </summary>
        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Category ID must be greater than 0", nameof(categoryId));

            return await _dbSet
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        /// <summary>
        /// Get all products by category name
        /// </summary>
        public async Task<IEnumerable<Product>> GetByCategoryNameAsync(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                throw new ArgumentException("Category name cannot be null or empty", nameof(categoryName));

            return await _dbSet
                .Where(p => p.Category != null && p.Category.Name == categoryName)
                .ToListAsync();
        }

        /// <summary>
        /// Get products by partial name match (case-insensitive)
        /// </summary>
        public async Task<IEnumerable<Product>> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty", nameof(name));

            return await _dbSet
                .Where(p => p.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();
        }

        /// <summary>
        /// Get product with full detail information for product detail page
        /// </summary>
        public async Task<Product?> GetProductWithCategoryAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(id));

            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .Include(p => p.ProductOptions)
                    .ThenInclude(o => o.ProductOptionValues)
                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.VariantOptionValues)
                        .ThenInclude(vov => vov.ProductOptionValue)
                            .ThenInclude(pov => pov.ProductOption)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Get all products with brand and category loaded
        /// </summary>
        public async Task<IEnumerable<Product>> GetAllWithBrandAndCategoryAsync()
        {
            return await _dbSet
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToListAsync();
        }
    }
}
