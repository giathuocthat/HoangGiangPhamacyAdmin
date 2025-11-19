using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository implementation for Category entity
    /// </summary>
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppContext context) : base(context)
        {
        }

        /// <summary>
        /// Get category by exact name match
        /// </summary>
        public async Task<Category?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be null or empty", nameof(name));

            return await _dbSet
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        /// <summary>
        /// Search categories by partial name match (case-insensitive)
        /// </summary>
        public async Task<IEnumerable<Category>> SearchByNameAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Search term cannot be null or empty", nameof(searchTerm));

            return await _dbSet
                .Where(c => c.Name.ToLower().Contains(searchTerm.ToLower()))
                .ToListAsync();
        }
    }
}
