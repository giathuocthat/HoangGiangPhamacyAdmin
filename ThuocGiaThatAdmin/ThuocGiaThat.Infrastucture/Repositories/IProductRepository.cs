using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository interface for Product entity
    /// </summary>
    public interface IProductRepository : IRepository<Product>
    {
        /// <summary>
        /// Get products by category ID
        /// </summary>
        Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);

        /// <summary>
        /// Get products by category name
        /// </summary>
        Task<IEnumerable<Product>> GetByCategoryNameAsync(string categoryName);

        /// <summary>
        /// Get products by name (partial match)
        /// </summary>
        Task<IEnumerable<Product>> GetByNameAsync(string name);

        /// <summary>
        /// Get product with category information
        /// </summary>
        Task<Product?> GetProductWithCategoryAsync(int id);

        /// <summary>
        /// Get all products including brand and category
        /// </summary>
        Task<IEnumerable<Product>> GetAllWithBrandAndCategoryAsync();

        /// <summary>
        /// Get Products with Paging
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<(IList<Product> products, int TotalCount)> GetPagedProductsAsync(string? category, string? price, int? type, string? sort, int page = 1, int pageSize = 20);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        Task<Product?> GetProductWithCategoryAsync(string slug);
    }
}
