using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Generic repository base class with common database operations
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly TrueMecContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(TrueMecContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        #region Add Operations

        /// <summary>
        /// Add a single entity to the database
        /// </summary>
        public virtual async Task AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _dbSet.AddAsync(entity);
        }

        /// <summary>
        /// Add multiple entities to the database
        /// </summary>
        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await _dbSet.AddRangeAsync(entities);
        }

        public  void AddRange(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

             _dbSet.AddRange(entities);
        }

        #endregion

        #region Update Operations

        /// <summary>
        /// Update a single entity
        /// </summary>
        public virtual void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Update(entity);
        }

        /// <summary>
        /// Update multiple entities
        /// </summary>
        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            _dbSet.UpdateRange(entities);
        }

        #endregion

        #region Delete Operations

        /// <summary>
        /// Delete a single entity
        /// </summary>
        public virtual void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Remove(entity);
        }

        /// <summary>
        /// Delete multiple entities
        /// </summary>
        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            _dbSet.RemoveRange(entities);
        }

        #endregion

        #region Read Operations

        /// <summary>
        /// Get entity by ID
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(int id)
        {
           return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Get all entities
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();
            foreach (var navigation in includes)
            {
                query = query.Include(navigation);
            }
            return query.ToList();
        }

        /// <summary>
        /// Filter entities based on predicate (condition)
        /// </summary>
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Get first entity matching predicate, or null if not found
        /// </summary>
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Check if any entity matches the predicate
        /// </summary>
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await _dbSet.AnyAsync(predicate);
        }

        public virtual async Task<ThuocGiaThat.Infrastucture.Common.PagedResult<T>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>> predicate = null,
            string sortField = null,
            string sortOrder = "asc",
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            // Sorting
            if (!string.IsNullOrEmpty(sortField))
            {
                // Using System.Linq.Dynamic.Core
                // Ensure proper casing or just pass as is if package handles it (it usually does)
                // Defaulting to "asc" if sortOrder is null or empty
                string order = string.IsNullOrEmpty(sortOrder) ? "asc" : sortOrder;
                query = System.Linq.Dynamic.Core.DynamicQueryableExtensions.OrderBy(query, $"{sortField} {order}");
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new ThuocGiaThat.Infrastucture.Common.PagedResult<T>(items, totalCount, pageNumber, pageSize);
        }

        #endregion

        #region Save Changes

        /// <summary>
        /// Save changes to the database
        /// </summary>
        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        #endregion
    }
}
