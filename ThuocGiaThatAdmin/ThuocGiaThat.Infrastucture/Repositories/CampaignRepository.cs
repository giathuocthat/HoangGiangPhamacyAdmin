using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository implementation for Campaign entity
    /// </summary>
    public class CampaignRepository : Repository<Campaign>, ICampaignRepository
    {
        public CampaignRepository(TrueMecContext context) : base(context)
        {
        }

        /// <summary>
        /// Get campaign by code
        /// </summary>
        public async Task<Campaign?> GetByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Campaign code cannot be null or empty", nameof(code));

            return await _dbSet
                .FirstOrDefaultAsync(c => c.CampaignCode == code);
        }

        /// <summary>
        /// Get campaign with all related banners
        /// </summary>
        public async Task<Campaign?> GetWithBannersAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Campaign ID must be greater than 0", nameof(id));

            return await _dbSet
                .Include(c => c.Banners)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Get all active campaigns
        /// </summary>
        public async Task<IEnumerable<Campaign>> GetActiveCampaignsAsync()
        {
            var now = DateTime.Now;

            return await _dbSet
                .Where(c => c.IsActive && c.StartDate <= now && c.EndDate >= now)
                .OrderBy(c => c.StartDate)
                .ToListAsync();
        }

        /// <summary>
        /// Get campaigns by date range
        /// </summary>
        public async Task<IEnumerable<Campaign>> GetCampaignsByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _dbSet
                .Where(c => c.StartDate <= to && c.EndDate >= from)
                .OrderBy(c => c.StartDate)
                .ToListAsync();
        }

        /// <summary>
        /// Get paged campaigns
        /// </summary>
        public async Task<(IEnumerable<Campaign>, int totalCount)> GetPagedCampaignsAsync(int pageNumber = 1, int pageSize = 20)
        {
            var query = _dbSet
                .Include(c => c.Banners)
                .AsQueryable()
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var campaigns = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (campaigns, totalCount);
        }

        /// <summary>
        /// Check if campaign code exists
        /// </summary>
        public async Task<bool> CodeExistsAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            return await _dbSet.AnyAsync(c => c.CampaignCode == code);
        }
    }
}
