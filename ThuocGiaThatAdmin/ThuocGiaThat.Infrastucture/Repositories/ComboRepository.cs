using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository implementation for Combo entity
    /// </summary>
    public class ComboRepository : Repository<Combo>, IComboRepository
    {
        public ComboRepository(TrueMecContext context) : base(context)
        {
        }

        /// <summary>
        /// Get combo by code
        /// </summary>
        public async Task<Combo?> GetByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Combo code cannot be null or empty", nameof(code));

            return await _dbSet
                .FirstOrDefaultAsync(c => c.ComboCode == code);
        }

        /// <summary>
        /// Get combo with all items and product details
        /// </summary>
        public async Task<Combo?> GetWithItemsAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Combo ID must be greater than 0", nameof(id));

            return await _dbSet
                .Include(c => c.Banner)
                .Include(c => c.ComboItems)
                    .ThenInclude(ci => ci.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Get all active combos
        /// </summary>
        public async Task<IEnumerable<Combo>> GetActiveCombosAsync()
        {
            var now = DateTime.Now;

            return await _dbSet
                .Where(c => c.IsActive && c.ValidFrom <= now && c.ValidTo >= now)
                .Include(c => c.ComboItems)
                    .ThenInclude(ci => ci.ProductVariant)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// Get combos by banner
        /// </summary>
        public async Task<IEnumerable<Combo>> GetByBannerAsync(int bannerId)
        {
            if (bannerId <= 0)
                throw new ArgumentException("Banner ID must be greater than 0", nameof(bannerId));

            return await _dbSet
                .Where(c => c.BannerId == bannerId)
                .Include(c => c.ComboItems)
                    .ThenInclude(ci => ci.ProductVariant)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// Get paged combos
        /// </summary>
        public async Task<(IEnumerable<Combo>, int totalCount)> GetPagedCombosAsync(int pageNumber = 1, int pageSize = 20)
        {
            var query = _dbSet
                .Include(c => c.Banner)
                .Include(c => c.ComboItems)
                .AsQueryable()
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var combos = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (combos, totalCount);
        }

        /// <summary>
        /// Check if combo code exists
        /// </summary>
        public async Task<bool> CodeExistsAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            return await _dbSet.AnyAsync(c => c.ComboCode == code);
        }

        /// <summary>
        /// Check stock availability for combo
        /// </summary>
        public async Task<bool> CheckStockAvailabilityAsync(int comboId, int quantity)
        {
            if (comboId <= 0)
                throw new ArgumentException("Combo ID must be greater than 0", nameof(comboId));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0", nameof(quantity));

            var combo = await _dbSet
                .Include(c => c.ComboItems)
                    .ThenInclude(ci => ci.ProductVariant)
                        .ThenInclude(pv => pv.Inventories)
                .FirstOrDefaultAsync(c => c.Id == comboId);

            if (combo == null)
                return false;

            // Check if all items have sufficient stock
            foreach (var item in combo.ComboItems)
            {
                var requiredQuantity = item.Quantity * quantity;
                var availableStock = item.ProductVariant?.Inventories?.Sum(i => i.QuantityOnHand) ?? 0;

                if (availableStock < requiredQuantity)
                    return false;
            }

            return true;
        }
    }
}
