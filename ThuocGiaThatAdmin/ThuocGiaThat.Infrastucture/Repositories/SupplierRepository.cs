using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository implementation for Supplier entity
    /// </summary>
    public class SupplierRepository : Repository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(TrueMecContext context) : base(context)
        {
        }

        public async Task<Supplier?> GetByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code cannot be null or empty", nameof(code));

            return await _dbSet
                .FirstOrDefaultAsync(s => s.Code == code);
        }

        public async Task<Supplier?> GetWithContactsAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Supplier ID must be greater than 0", nameof(id));

            return await _dbSet
                .Include(s => s.SupplierContacts.Where(c => c.IsActive))
                .Include(s => s.Ward)
                .Include(s => s.Province)
                .Include(s => s.Bank)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Supplier>> GetActiveSuppliersAsync()
        {
            return await _dbSet
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<(IList<Supplier> suppliers, int totalCount)> GetPagedSuppliersAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            bool? isActive = null)
        {
            var query = _dbSet.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(s =>
                    s.Name.Contains(searchTerm) ||
                    s.Code.Contains(searchTerm) ||
                    (s.TaxCode != null && s.TaxCode.Contains(searchTerm)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();

            var suppliers = await query
                .Include(s => s.Ward)
                .Include(s => s.Province)
                .Include(s => s.Bank)
                .OrderByDescending(s => s.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (suppliers, totalCount);
        }

        public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            var query = _dbSet.Where(s => s.Code == code);

            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }

    /// <summary>
    /// Repository implementation for SupplierContact entity
    /// </summary>
    public class SupplierContactRepository : Repository<SupplierContact>, ISupplierContactRepository
    {
        public SupplierContactRepository(TrueMecContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SupplierContact>> GetBySupplierIdAsync(int supplierId)
        {
            if (supplierId <= 0)
                throw new ArgumentException("Supplier ID must be greater than 0", nameof(supplierId));

            return await _dbSet
                .Where(c => c.SupplierId == supplierId)
                .OrderByDescending(c => c.IsPrimary)
                .ThenBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<SupplierContact?> GetPrimaryContactAsync(int supplierId)
        {
            if (supplierId <= 0)
                throw new ArgumentException("Supplier ID must be greater than 0", nameof(supplierId));

            return await _dbSet
                .Where(c => c.SupplierId == supplierId && c.IsPrimary && c.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SupplierContact>> GetActiveContactsBySupplierIdAsync(int supplierId)
        {
            if (supplierId <= 0)
                throw new ArgumentException("Supplier ID must be greater than 0", nameof(supplierId));

            return await _dbSet
                .Where(c => c.SupplierId == supplierId && c.IsActive)
                .OrderByDescending(c => c.IsPrimary)
                .ThenBy(c => c.FullName)
                .ToListAsync();
        }
    }
}
