using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository interface for Banner entity
    /// </summary>
    public interface IBannerRepository : IRepository<Banner>
    {
        /// <summary>
        /// Get banner by code
        /// </summary>
        Task<Banner?> GetByCodeAsync(string code);

        /// <summary>
        /// Get banner with all related data (sections, combos, vouchers, product variants)
        /// </summary>
        Task<Banner?> GetWithDetailsAsync(int id);

        /// <summary>
        /// Get all active banners
        /// </summary>
        Task<IEnumerable<Banner>> GetActiveBannersAsync();

        /// <summary>
        /// Get banners for slider display (active, within date range, ordered)
        /// </summary>
        Task<IEnumerable<Banner>> GetBannerSliderAsync(int? campaignId = null, int maxCount = 8);

        /// <summary>
        /// Get banners by campaign
        /// </summary>
        Task<IEnumerable<Banner>> GetByCampaignAsync(int campaignId);

        /// <summary>
        /// Get paged banners
        /// </summary>
        Task<(IEnumerable<Banner>, int totalCount)> GetPagedBannersAsync(int pageNumber = 1, int pageSize = 20);

        /// <summary>
        /// Increment view count
        /// </summary>
        Task IncrementViewCountAsync(int id);

        /// <summary>
        /// Increment click count
        /// </summary>
        Task IncrementClickCountAsync(int id);

        /// <summary>
        /// Check if banner code exists
        /// </summary>
        Task<bool> CodeExistsAsync(string code);
    }
}
