using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository implementation for CustomerPaymentAccount entity
    /// </summary>
    public class CustomerPaymentAccountRepository : Repository<CustomerPaymentAccount>, ICustomerPaymentAccountRepository
    {
        public CustomerPaymentAccountRepository(TrueMecContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CustomerPaymentAccountDto>> GetAllAsync()
        {
            return await _dbSet
                .Include(a => a.Customer)
                .Include(a => a.Bank)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedDate)
                .Select(a => MapToDto(a))
                .ToListAsync();
        }

        public new async Task<CustomerPaymentAccountDto?> GetByIdAsync(int id)
        {
            var account = await _dbSet
                .Include(a => a.Customer)
                .Include(a => a.Bank)
                .FirstOrDefaultAsync(a => a.Id == id);

            return account != null ? MapToDto(account) : null;
        }

        public async Task<IEnumerable<CustomerPaymentAccountDto>> GetByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .Include(a => a.Customer)
                .Include(a => a.Bank)
                .Where(a => a.CustomerId == customerId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedDate)
                .Select(a => MapToDto(a))
                .ToListAsync();
        }

        public async Task<CustomerPaymentAccountDto?> GetDefaultAccountAsync(int customerId)
        {
            var account = await _dbSet
                .Include(a => a.Customer)
                .Include(a => a.Bank)
                .FirstOrDefaultAsync(a => a.CustomerId == customerId && a.IsDefault && a.IsActive);

            return account != null ? MapToDto(account) : null;
        }

        public async Task<CustomerPaymentAccountDto> CreateAsync(CustomerPaymentAccountDto dto)
        {
            var account = new CustomerPaymentAccount
            {
                CustomerId = dto.CustomerId,
                AccountType = dto.AccountType,
                BankId = dto.BankId,
                BankName = dto.BankName ?? string.Empty,
                AccountNumber = dto.AccountNumber,
                AccountHolder = dto.AccountHolder,
                BankBranch = dto.BankBranch,
                SwiftCode = dto.SwiftCode,
                IsDefault = dto.IsDefault,
                IsActive = dto.IsActive,
                Notes = dto.Notes
            };

            await _dbSet.AddAsync(account);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(account.Id) ?? dto;
        }

        public async Task<CustomerPaymentAccountDto> UpdateAsync(CustomerPaymentAccountDto dto)
        {
            var account = await _dbSet.FindAsync(dto.Id);
            if (account == null)
                throw new InvalidOperationException($"CustomerPaymentAccount with ID {dto.Id} not found");

            account.AccountType = dto.AccountType;
            account.BankId = dto.BankId;
            account.BankName = dto.BankName ?? string.Empty;
            account.AccountNumber = dto.AccountNumber;
            account.AccountHolder = dto.AccountHolder;
            account.BankBranch = dto.BankBranch;
            account.SwiftCode = dto.SwiftCode;
            account.IsActive = dto.IsActive;
            account.Notes = dto.Notes;

            _dbSet.Update(account);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(account.Id) ?? dto;
        }

        public async Task DeleteAsync(int id)
        {
            var account = await _dbSet.FindAsync(id);
            if (account == null)
                throw new InvalidOperationException($"CustomerPaymentAccount with ID {id} not found");

            _dbSet.Remove(account);
            await _context.SaveChangesAsync();
        }

        public async Task SetDefaultAccountAsync(int accountId, int customerId)
        {
            // Unset all default accounts for this customer
            var currentDefaults = await _dbSet
                .Where(a => a.CustomerId == customerId && a.IsDefault)
                .ToListAsync();

            foreach (var account in currentDefaults)
            {
                account.IsDefault = false;
            }

            // Set the new default account
            var newDefault = await _dbSet.FindAsync(accountId);
            if (newDefault == null)
                throw new InvalidOperationException($"CustomerPaymentAccount with ID {accountId} not found");

            if (newDefault.CustomerId != customerId)
                throw new InvalidOperationException($"Account {accountId} does not belong to customer {customerId}");

            newDefault.IsDefault = true;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> AccountExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(a => a.Id == id);
        }

        private static CustomerPaymentAccountDto MapToDto(CustomerPaymentAccount account)
        {
            return new CustomerPaymentAccountDto
            {
                Id = account.Id,
                CustomerId = account.CustomerId,
                CustomerName = account.Customer?.FullName,
                AccountType = account.AccountType,
                BankId = account.BankId,
                BankName = account.Bank?.BankName,
                AccountNumber = account.AccountNumber,
                AccountHolder = account.AccountHolder,
                BankBranch = account.BankBranch,
                SwiftCode = account.SwiftCode,
                IsDefault = account.IsDefault,
                IsActive = account.IsActive,
                Notes = account.Notes,
                CreatedDate = account.CreatedDate,
                UpdatedDate = account.UpdatedDate
            };
        }
    }
}
