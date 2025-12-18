using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository implementation for Bank entity
    /// </summary>
    public class BankRepository : Repository<Bank>, IBankRepository
    {
        public BankRepository(TrueMecContext context) : base(context)
        {
        }

        public async Task<List<BankDto>> GetAllBanksAsync()
        {
            return await _context.Set<Bank>()
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.BankName)
                .Select(x => new BankDto
                {
                    Id = x.Id,
                    BankCode = x.BankCode,
                    BankName = x.BankName,
                    FullName = x.FullName,
                    IsActive = x.IsActive,
                    DisplayOrder = x.DisplayOrder,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate
                })
                .ToListAsync();
        }

        public async Task<List<BankDto>> GetActiveBanksAsync()
        {
            return await _context.Set<Bank>()
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.BankName)
                .Select(x => new BankDto
                {
                    Id = x.Id,
                    BankCode = x.BankCode,
                    BankName = x.BankName,
                    FullName = x.FullName,
                    IsActive = x.IsActive,
                    DisplayOrder = x.DisplayOrder,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate
                })
                .ToListAsync();
        }

        public async Task<BankDto?> GetBankByIdAsync(int id)
        {
            return await _context.Set<Bank>()
                .Where(x => x.Id == id)
                .Select(x => new BankDto
                {
                    Id = x.Id,
                    BankCode = x.BankCode,
                    BankName = x.BankName,
                    FullName = x.FullName,
                    IsActive = x.IsActive,
                    DisplayOrder = x.DisplayOrder,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate
                })
                .FirstOrDefaultAsync();
        }

        public async Task<BankDto?> GetBankByCodeAsync(string bankCode)
        {
            return await _context.Set<Bank>()
                .Where(x => x.BankCode == bankCode)
                .Select(x => new BankDto
                {
                    Id = x.Id,
                    BankCode = x.BankCode,
                    BankName = x.BankName,
                    FullName = x.FullName,
                    IsActive = x.IsActive,
                    DisplayOrder = x.DisplayOrder,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Bank> CreateAsync(Bank bank)
        {
            await _context.Set<Bank>().AddAsync(bank);
            await _context.SaveChangesAsync();
            return bank;
        }

        public async Task<Bank> UpdateAsync(Bank bank)
        {
            _context.Set<Bank>().Update(bank);
            await _context.SaveChangesAsync();
            return bank;
        }

        public async Task DeleteAsync(int id)
        {
            var bank = await _context.Set<Bank>().FindAsync(id);
            if (bank != null)
            {
                _context.Set<Bank>().Remove(bank);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> BankCodeExistsAsync(string bankCode, int? excludeId = null)
        {
            var query = _context.Set<Bank>().Where(x => x.BankCode == bankCode);

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
