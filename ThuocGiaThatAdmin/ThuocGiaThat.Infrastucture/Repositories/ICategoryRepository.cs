using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository interface for Category entity
    /// </summary>
    public interface ICategoryRepository : IRepository<Category>
    {
        /// <summary>
        /// Get category by name
        /// </summary>
        Task<Category?> GetByNameAsync(string name);

        /// <summary>
        /// Get categories by partial name match
        /// </summary>
        Task<IEnumerable<Category>> SearchByNameAsync(string searchTerm);
    }
}
