using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public class SalesRegionRepository : ISalesRegionRepository
    {
        private readonly TrueMecContext _context;

        public SalesRegionRepository(TrueMecContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalesRegion>> GetAllActiveAsync()
        {
            return await _context.SalesRegions
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<SalesRegion?> GetByIdAsync(int id)
        {
            return await _context.SalesRegions
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<SalesRegion?> GetByCodeAsync(string code)
        {
            return await _context.SalesRegions
                .FirstOrDefaultAsync(r => r.Code == code);
        }

        public async Task<SalesRegion?> GetRegionWithUsersAsync(int id)
        {
            return await _context.SalesRegions
                .Include(r => r.SalesUsers)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<SalesRegion?> GetRegionWithCustomersAsync(int id)
        {
            return await _context.SalesRegions
                .Include(r => r.Customers)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<SalesRegion> AddAsync(SalesRegion region)
        {
            await _context.SalesRegions.AddAsync(region);
            return region;
        }

        public void Update(SalesRegion region)
        {
            _context.SalesRegions.Update(region);
        }

        public void Delete(SalesRegion region)
        {
            _context.SalesRegions.Remove(region);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
        {
            var query = _context.SalesRegions.Where(r => r.Code == code);

            if (excludeId.HasValue)
            {
                query = query.Where(r => r.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
