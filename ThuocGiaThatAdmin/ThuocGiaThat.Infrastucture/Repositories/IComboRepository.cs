using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository interface for Combo entity
    /// </summary>
    public interface IComboRepository : IRepository<Combo>
    {
        /// <summary>
        /// Get combo by code
        /// </summary>
        Task<Combo?> GetByCodeAsync(string code);

        /// <summary>
        /// Get combo with all items and product details
        /// </summary>
        Task<Combo?> GetWithItemsAsync(int id);

        /// <summary>
        /// Get all active combos
        /// </summary>
        Task<IEnumerable<Combo>> GetActiveCombosAsync();

        /// <summary>
        /// Get combos by banner
        /// </summary>
        Task<IEnumerable<Combo>> GetByBannerAsync(int bannerId);

        /// <summary>
        /// Get paged combos
        /// </summary>
        Task<(IEnumerable<Combo>, int totalCount)> GetPagedCombosAsync(int pageNumber = 1, int pageSize = 20);

        /// <summary>
        /// Check if combo code exists
        /// </summary>
        Task<bool> CodeExistsAsync(string code);

        /// <summary>
        /// Check stock availability for combo
        /// </summary>
        Task<bool> CheckStockAvailabilityAsync(int comboId, int quantity);
    }
}
