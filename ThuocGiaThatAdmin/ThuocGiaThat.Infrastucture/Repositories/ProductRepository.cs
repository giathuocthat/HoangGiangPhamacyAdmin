using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Contract.DTOs;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository implementation for Product entity
    /// </summary>
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(TrueMecContext context) : base(context)
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
                .Include(p => p.Images)
                .Include(p => p.ProductVariants)
                .ToListAsync();
        }

        /// <summary>
        /// Get brands with pagination
        /// </summary>
        public async Task<(IList<Product> products, int TotalCount)> GetPagedProductsAsync(string? category, string? price, int? type, string? sort, int page = 1, int pageSize = 20)
        {
            var query = _context.Set<Product>()
                .Include(x => x.Brand)
                .Include(x => x.Category)
                .Where(x => x.IsActive)
                .AsQueryable()
                .AsNoTracking();

            if(!string.IsNullOrEmpty(category))
            {
                var categoryId = await _context.Set<Category>().Where(x => x.Slug == category).Select(x => x.Id).FirstOrDefaultAsync();
                if (categoryId != 0)
                {
                    query = query.Where(x => x.CategoryId == categoryId);
                }
            }

            if (!string.IsNullOrEmpty(price))
            {
                switch(price)
                {
                    case "lt_100k":
                        query = query.Where(x => x.ProductVariants.Any(y => y.Price <= 100000));
                        break;
                    case "100k_200k":
                        query = query.Where(x => x.ProductVariants.Any(y => y.Price > 100000 && y.Price <= 200000));
                        break;
                    case "200k_300k":
                        query = query.Where(x => x.ProductVariants.Any(y => y.Price > 200000 && y.Price <= 300000));
                        break;
                    case "gt_300k":
                        query.Where(x => x.ProductVariants.Any(y => y.Price > 300000));
                        break;
                }
            }

            if (type.HasValue)
            {
                //query = query.Where(x => x.ProductType == type);
            }

            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "price_asc":
                        query = query.OrderBy(p => p.ProductVariants.Min(v => v.Price));
                        break;
                    case "price_desc":
                        query = query.OrderByDescending(p => p.ProductVariants.Max(v => v.Price));
                        break;
                    case "name_asc":
                        query = query.OrderBy(p => p.Name);
                        break;
                    case "name_desc":
                        query = query.OrderByDescending(p => p.Name);
                        break;
                    case "newest":
                        query = query.OrderByDescending(p => p.CreatedDate);
                        break;
                    default:
                        query = query.OrderBy(p => p.Id);
                        break;
                }
            }

            var totalCount = await query.CountAsync();

            var products = await query
                .Include(p => p.ProductVariants.Take(1))                
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }

        /// <summary>
        /// Get Product By slug
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<Product?> GetProductWithCategoryAsync(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                throw new ArgumentException("slug empty");

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
                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.Inventories)
                .FirstOrDefaultAsync(p => p.Slug.Equals(slug));
        }
    }
}
