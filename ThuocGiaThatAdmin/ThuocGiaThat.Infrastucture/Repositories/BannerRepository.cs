using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository implementation for Banner entity
    /// </summary>
    public class BannerRepository : Repository<Banner>, IBannerRepository
    {
        public BannerRepository(TrueMecContext context) : base(context)
        {
        }

        /// <summary>
        /// Get banner by code
        /// </summary>
        public async Task<Banner?> GetByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Banner code cannot be null or empty", nameof(code));

            return await _dbSet
                .FirstOrDefaultAsync(b => b.BannerCode == code);
        }

        /// <summary>
        /// Get banner with all related data
        /// </summary>
        public async Task<Banner?> GetWithDetailsAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Banner ID must be greater than 0", nameof(id));

            return await _dbSet
                .Include(b => b.Campaign)
                .Include(b => b.BannerSections)
                .Include(b => b.Combos)
                    .ThenInclude(c => c.ComboItems)
                        .ThenInclude(ci => ci.ProductVariant)
                .Include(b => b.Vouchers)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        /// <summary>
        /// Get all active banners
        /// </summary>
        public async Task<IEnumerable<Banner>> GetActiveBannersAsync()
        {
            var now = DateTime.Now;

            return await _dbSet
                .Where(b => b.IsActive && b.ValidFrom <= now && b.ValidTo >= now)
                .OrderBy(b => b.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// Get banners for slider display
        /// </summary>
        public async Task<IEnumerable<Banner>> GetBannerSliderAsync(int? campaignId = null, int maxCount = 8)
        {
            var now = DateTime.Now;
            var query = _dbSet
                .Where(b => b.IsActive && b.ValidFrom <= now && b.ValidTo >= now);

            if (campaignId.HasValue)
            {
                query = query.Where(b => b.CampaignId == campaignId.Value);
            }

            return await query
                .Include(b => b.BannerSections)
                .OrderBy(b => b.DisplayOrder)
                .Take(maxCount)
                .ToListAsync();
        }

        /// <summary>
        /// Get banners by campaign
        /// </summary>
        public async Task<IEnumerable<Banner>> GetByCampaignAsync(int campaignId)
        {
            if (campaignId <= 0)
                throw new ArgumentException("Campaign ID must be greater than 0", nameof(campaignId));

            return await _dbSet
                .Where(b => b.CampaignId == campaignId)
                .OrderBy(b => b.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// Get paged banners
        /// </summary>
        public async Task<(IEnumerable<Banner>, int totalCount)> GetPagedBannersAsync(int pageNumber = 1, int pageSize = 20)
        {
            var query = _dbSet
                .Include(b => b.Campaign)
                .AsQueryable()
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var banners = await query
                .OrderByDescending(b => b.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (banners, totalCount);
        }

        /// <summary>
        /// Increment view count
        /// </summary>
        public async Task IncrementViewCountAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Banner ID must be greater than 0", nameof(id));

            var banner = await _dbSet.FindAsync(id);
            if (banner != null)
            {
                banner.ViewCount++;
                _dbSet.Update(banner);
            }
        }

        /// <summary>
        /// Increment click count
        /// </summary>
        public async Task IncrementClickCountAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Banner ID must be greater than 0", nameof(id));

            var banner = await _dbSet.FindAsync(id);
            if (banner != null)
            {
                banner.ClickCount++;
                _dbSet.Update(banner);
            }
        }

        /// <summary>
        /// Check if banner code exists
        /// </summary>
        public async Task<bool> CodeExistsAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            return await _dbSet.AnyAsync(b => b.BannerCode == code);
        }
    }
}
