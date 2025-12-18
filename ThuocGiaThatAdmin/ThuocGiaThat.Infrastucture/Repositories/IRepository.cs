using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Generic repository interface for common database operations
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        // Add operations
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void AddRange(IEnumerable<T> entities);

        // Update operations
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

        // Delete operations
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);

        // Read operations
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        // Save changes
        Task<int> SaveChangesAsync();
    }
}
