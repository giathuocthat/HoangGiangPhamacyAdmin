using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository interface for Voucher entity
    /// </summary>
    public interface IVoucherRepository : IRepository<Voucher>
    {
        /// <summary>
        /// Get voucher by code
        /// </summary>
        Task<Voucher?> GetByCodeAsync(string code);

        /// <summary>
        /// Get voucher with all related data (categories, product variants)
        /// </summary>
        Task<Voucher?> GetByIdWithDetailsAsync(int id);

        /// <summary>
        /// Get voucher by code with all related data
        /// </summary>
        Task<Voucher?> GetByCodeWithDetailsAsync(string code);

        /// <summary>
        /// Get all active vouchers
        /// </summary>
        Task<IEnumerable<Voucher>> GetActiveVouchersAsync();

        /// <summary>
        /// Get vouchers applicable to a specific category
        /// </summary>
        Task<IEnumerable<Voucher>> GetVouchersByCategoryAsync(int categoryId);

        /// <summary>
        /// Get vouchers applicable to a specific product variant
        /// </summary>
        Task<IEnumerable<Voucher>> GetVouchersByProductVariantAsync(int productVariantId);

        /// <summary>
        /// Get user's voucher usage count for a specific voucher
        /// </summary>
        Task<int> GetUserVoucherUsageCountAsync(int voucherId, string userId);

        /// <summary>
        /// Get stackable vouchers (can be used with other vouchers)
        /// </summary>
        Task<IEnumerable<Voucher>> GetStackableVouchersAsync();

        /// <summary>
        /// Get vouchers by multiple codes
        /// </summary>
        Task<IEnumerable<Voucher>> GetByCodesAsync(List<string> codes);

        /// <summary>
        /// Check if voucher code exists
        /// </summary>
        Task<bool> CodeExistsAsync(string code);

        /// <summary>
        /// Get voucher usage history for a specific voucher
        /// </summary>
        Task<IEnumerable<VoucherUsageHistory>> GetUsageHistoryAsync(int voucherId, int pageNumber = 1, int pageSize = 20);

        /// <summary>
        /// Get voucher usage history for a specific user
        /// </summary>
        Task<IEnumerable<VoucherUsageHistory>> GetUserUsageHistoryAsync(string userId, int pageNumber = 1, int pageSize = 20);

        /// <summary>
        /// Increment voucher usage count
        /// </summary>
        Task IncrementUsageCountAsync(int voucherId);
    }
}
