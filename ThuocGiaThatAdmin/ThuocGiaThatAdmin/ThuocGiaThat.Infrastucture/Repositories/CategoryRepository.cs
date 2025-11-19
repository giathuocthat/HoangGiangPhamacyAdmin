using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetCategoryWithProductsAsync(int categoryId);
    }

    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppContext context) : base(context)
        {
        }

        public async Task<Category?> GetCategoryWithProductsAsync(int categoryId)
        {
            return await _dbSet
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        public override async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _dbSet.Include(c => c.Products).ToListAsync();
        }
    }
}
