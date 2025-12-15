using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository interface for Campaign entity
    /// </summary>
    public interface ICampaignRepository : IRepository<Campaign>
    {
        /// <summary>
        /// Get campaign by code
        /// </summary>
        Task<Campaign?> GetByCodeAsync(string code);

        /// <summary>
        /// Get campaign with all related banners
        /// </summary>
        Task<Campaign?> GetWithBannersAsync(int id);

        /// <summary>
        /// Get all active campaigns
        /// </summary>
        Task<IEnumerable<Campaign>> GetActiveCampaignsAsync();

        /// <summary>
        /// Get campaigns by date range
        /// </summary>
        Task<IEnumerable<Campaign>> GetCampaignsByDateRangeAsync(DateTime from, DateTime to);

        /// <summary>
        /// Get paged campaigns
        /// </summary>
        Task<(IEnumerable<Campaign>, int totalCount)> GetPagedCampaignsAsync(int pageNumber = 1, int pageSize = 20);

        /// <summary>
        /// Check if campaign code exists
        /// </summary>
        Task<bool> CodeExistsAsync(string code);
    }
}
