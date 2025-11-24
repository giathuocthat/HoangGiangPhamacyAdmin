using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository interface for Brand entity operations
    /// </summary>
    public interface IBrandRepository : IRepository<Brand>
    {
        /// <summary>
        /// Get brands with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Tuple containing list of brands and total count</returns>
        Task<(IEnumerable<Brand> Brands, int TotalCount)> GetBrandsWithPagingAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Get brand by slug
        /// </summary>
        Task<Brand?> GetBySlugAsync(string slug);

        /// <summary>
        /// Check if brand has any products
        /// </summary>
        /// <param name="brandId">Brand ID</param>
        /// <returns>True if brand has products, false otherwise</returns>
        Task<bool> HasProductsAsync(int brandId);
    }
}
