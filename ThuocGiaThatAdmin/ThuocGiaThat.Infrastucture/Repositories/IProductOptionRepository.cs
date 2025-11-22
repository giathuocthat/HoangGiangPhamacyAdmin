using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository interface for ProductOption entity operations
    /// </summary>
    public interface IProductOptionRepository : IRepository<ProductOption>
    {
        /// <summary>
        /// Get product options with pagination, including their values
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Tuple containing list of product options and total count</returns>
        Task<(IEnumerable<ProductOption> ProductOptions, int TotalCount)> GetProductOptionsWithPagingAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Get product option by ID with its values
        /// </summary>
        Task<ProductOption?> GetByIdWithValuesAsync(int id);

        /// <summary>
        /// Check if product option is being used by any product
        /// </summary>
        Task<bool> IsUsedByProductAsync(int productOptionId);
    }
}
