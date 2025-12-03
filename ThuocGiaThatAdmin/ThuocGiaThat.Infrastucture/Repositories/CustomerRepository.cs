using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(TrueMecContext context) : base(context)
        {
        }

        /// <summary>
        /// Get customer by ID with related entities (BusinessType, Addresses)
        /// </summary>
        public async Task<Customer?> GetByIdWithIncludesAsync(int id)
        {
            return await _dbSet
                .Include(c => c.BusinessType)
                .Include(c => c.Addresses)
                .ThenInclude(a => a.Province)
                .Include(c => c.Addresses)
                .ThenInclude(a => a.Ward)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Get all customers with pagination and related entities
        /// </summary>
        public async Task<(IEnumerable<Customer> Customers, int TotalCount)> GetAllWithPaginationAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

            if (pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Page size must be between 1 and 100", nameof(pageSize));

            var query = _dbSet
                .Include(c => c.BusinessType)
                .Include(c => c.Addresses)
                    .ThenInclude(a => a.Province)
                .Include(c => c.Addresses)
                    .ThenInclude(a => a.Ward)
                .OrderByDescending(c => c.CreatedDate);

            var totalCount = await query.CountAsync();

            var customers = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (customers, totalCount);
        }

        /// <summary>
        /// Check if phone number exists (case-insensitive)
        /// </summary>
        public async Task<bool> IsPhoneNumberExistsAsync(string phoneNumber, int? excludeCustomerId = null)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            var query = _dbSet.Where(c => c.PhoneNumber.ToLower() == phoneNumber.ToLower());

            if (excludeCustomerId.HasValue)
            {
                query = query.Where(c => c.Id != excludeCustomerId.Value);
            }

            return await query.AnyAsync();
        }

        /// <summary>
        /// Check if email exists (case-insensitive)
        /// </summary>
        public async Task<bool> IsEmailExistsAsync(string email, int? excludeCustomerId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var query = _dbSet.Where(c => c.Email != null && c.Email.ToLower() == email.ToLower());

            if (excludeCustomerId.HasValue)
            {
                query = query.Where(c => c.Id != excludeCustomerId.Value);
            }

            return await query.AnyAsync();
        }

        /// <summary>
        /// Search customers by phone number (partial match)
        /// </summary>
        public async Task<IEnumerable<Customer>> SearchByPhoneAsync(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return new List<Customer>();

            return await _dbSet
                .Include(c => c.BusinessType)
                .Include(c => c.Addresses)
                    .ThenInclude(a => a.Province)
                .Include(c => c.Addresses)
                    .ThenInclude(a => a.Ward)
                .Where(c => c.PhoneNumber.Contains(phoneNumber))
                .OrderByDescending(c => c.CreatedDate)
                .Take(10) // Limit results to 10
                .ToListAsync();
        }
    }
}
