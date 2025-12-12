using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository implementation for Voucher entity
    /// </summary>
    public class VoucherRepository : Repository<Voucher>, IVoucherRepository
    {
        public VoucherRepository(TrueMecContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Voucher?>> GetAll()
        {

            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Get voucher by code
        /// </summary>
        public async Task<Voucher?> GetByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Voucher code cannot be null or empty", nameof(code));

            return await _dbSet
                .FirstOrDefaultAsync(v => v.Code == code);
        }

        /// <summary>
        /// Get voucher with all related data (categories, product variants)
        /// </summary>
        public async Task<Voucher?> GetByIdWithDetailsAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Voucher ID must be greater than 0", nameof(id));

            return await _dbSet
                .Include(v => v.VoucherCategories)
                    .ThenInclude(vc => vc.Category)
                .Include(v => v.VoucherProductVariants)
                    .ThenInclude(vpv => vpv.ProductVariant)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        /// <summary>
        /// Get voucher by code with all related data
        /// </summary>
        public async Task<Voucher?> GetByCodeWithDetailsAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Voucher code cannot be null or empty", nameof(code));

            return await _dbSet
                .Include(v => v.VoucherCategories)
                    .ThenInclude(vc => vc.Category)
                .Include(v => v.VoucherProductVariants)
                    .ThenInclude(vpv => vpv.ProductVariant)
                .FirstOrDefaultAsync(v => v.Code == code);
        }

        /// <summary>
        /// Get all active vouchers
        /// </summary>
        public async Task<IEnumerable<Voucher>> GetActiveVouchersAsync()
        {
            var now = DateTime.UtcNow;

            return await _dbSet
                .Where(v => v.IsActive && v.StartDate <= now && v.EndDate >= now)
                .Include(v => v.VoucherCategories)
                .Include(v => v.VoucherProductVariants)
                .ToListAsync();
        }

        /// <summary>
        /// Get vouchers applicable to a specific category
        /// </summary>
        public async Task<IEnumerable<Voucher>> GetVouchersByCategoryAsync(int categoryId)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Category ID must be greater than 0", nameof(categoryId));

            var now = DateTime.UtcNow;

            return await _dbSet
                .Where(v => v.IsActive
                    && v.StartDate <= now
                    && v.EndDate >= now
                    && (v.ApplicableType == VoucherApplicableType.All
                        || v.ApplicableType == VoucherApplicableType.Categories
                        || v.ApplicableType == VoucherApplicableType.Mixed)
                    && (v.ApplicableType == VoucherApplicableType.All
                        || v.VoucherCategories.Any(vc => vc.CategoryId == categoryId)))
                .Include(v => v.VoucherCategories)
                .Include(v => v.VoucherProductVariants)
                .ToListAsync();
        }

        /// <summary>
        /// Get vouchers applicable to a specific product variant
        /// </summary>
        public async Task<IEnumerable<Voucher>> GetVouchersByProductVariantAsync(int productVariantId)
        {
            if (productVariantId <= 0)
                throw new ArgumentException("Product variant ID must be greater than 0", nameof(productVariantId));

            var now = DateTime.UtcNow;

            return await _dbSet
                .Where(v => v.IsActive
                    && v.StartDate <= now
                    && v.EndDate >= now
                    && (v.ApplicableType == VoucherApplicableType.All
                        || v.ApplicableType == VoucherApplicableType.ProductVariants
                        || v.ApplicableType == VoucherApplicableType.Mixed)
                    && (v.ApplicableType == VoucherApplicableType.All
                        || v.VoucherProductVariants.Any(vpv => vpv.ProductVariantId == productVariantId)))
                .Include(v => v.VoucherCategories)
                .Include(v => v.VoucherProductVariants)
                .ToListAsync();
        }

        /// <summary>
        /// Get user's voucher usage count for a specific voucher
        /// </summary>
        public async Task<int> GetUserVoucherUsageCountAsync(int voucherId, string userId)
        {
            if (voucherId <= 0)
                throw new ArgumentException("Voucher ID must be greater than 0", nameof(voucherId));

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

            return await _context.VoucherUsageHistories
                .CountAsync(h => h.VoucherId == voucherId && h.UserId == userId);
        }

        /// <summary>
        /// Get stackable vouchers (can be used with other vouchers)
        /// </summary>
        public async Task<IEnumerable<Voucher>> GetStackableVouchersAsync()
        {
            var now = DateTime.UtcNow;

            return await _dbSet
                .Where(v => v.IsActive
                    && v.StartDate <= now
                    && v.EndDate >= now
                    && v.CanStackWithOthers)
                .Include(v => v.VoucherCategories)
                .Include(v => v.VoucherProductVariants)
                .OrderBy(v => v.StackPriority ?? int.MaxValue)
                .ToListAsync();
        }

        /// <summary>
        /// Get vouchers by multiple codes
        /// </summary>
        public async Task<IEnumerable<Voucher>> GetByCodesAsync(List<string> codes)
        {
            if (codes == null || !codes.Any())
                throw new ArgumentException("Codes list cannot be null or empty", nameof(codes));

            return await _dbSet
                .Where(v => codes.Contains(v.Code))
                .Include(v => v.VoucherCategories)
                .Include(v => v.VoucherProductVariants)
                .ToListAsync();
        }

        /// <summary>
        /// Check if voucher code exists
        /// </summary>
        public async Task<bool> CodeExistsAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            return await _dbSet.AnyAsync(v => v.Code == code);
        }

        /// <summary>
        /// Get voucher usage history for a specific voucher
        /// </summary>
        public async Task<IEnumerable<VoucherUsageHistory>> GetUsageHistoryAsync(int voucherId, int pageNumber = 1, int pageSize = 20)
        {
            if (voucherId <= 0)
                throw new ArgumentException("Voucher ID must be greater than 0", nameof(voucherId));

            return await _context.VoucherUsageHistories
                .Where(h => h.VoucherId == voucherId)
                .Include(h => h.User)
                .Include(h => h.Order)
                .OrderByDescending(h => h.UsedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Get voucher usage history for a specific user
        /// </summary>
        public async Task<IEnumerable<VoucherUsageHistory>> GetUserUsageHistoryAsync(string userId, int pageNumber = 1, int pageSize = 20)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

            return await _context.VoucherUsageHistories
                .Where(h => h.UserId == userId)
                .Include(h => h.Voucher)
                .Include(h => h.Order)
                .OrderByDescending(h => h.UsedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Increment voucher usage count
        /// </summary>
        public async Task IncrementUsageCountAsync(int voucherId)
        {
            if (voucherId <= 0)
                throw new ArgumentException("Voucher ID must be greater than 0", nameof(voucherId));

            var voucher = await _dbSet.FindAsync(voucherId);
            if (voucher != null)
            {
                voucher.CurrentUsageCount++;
                _dbSet.Update(voucher);
            }
        }

        public async Task<(IEnumerable<Voucher>, int totalCount)> GetPagedVoucher(int pageNumber = 1, int pageSize = 20)
        {

            var query = _context.Set<Voucher>()
                .Include(x => x.VoucherProductVariants)
                .Include(x => x.OrderVouchers)
                .Include(x => x.VoucherCategories)
                .AsQueryable()
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var vouchers = await query
                .OrderByDescending(b => b.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (vouchers, totalCount);
        }
    }
}
