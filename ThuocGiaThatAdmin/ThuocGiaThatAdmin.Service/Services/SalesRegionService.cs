using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class SalesRegionService : ISalesRegionService
    {
        private readonly ISalesRegionRepository _regionRepository;
        private readonly TrueMecContext _context;

        public SalesRegionService(ISalesRegionRepository regionRepository, TrueMecContext context)
        {
            _regionRepository = regionRepository;
            _context = context;
        }

        public async Task<IEnumerable<SalesRegionDto>> GetAllRegionsAsync()
        {
            var regions = await _context.SalesRegions
                .Include(r => r.SalesManager)
                .Include(r => r.SalesUsers)
                .Where(r => r.IsActive)
                .ToListAsync();

            return regions.Select(r => new SalesRegionDto
            {
                Id = r.Id,
                Name = r.Name,
                Code = r.Code,
                Description = r.Description,
                IsActive = r.IsActive,
                CreatedDate = r.CreatedDate,
                SaleManagerId = r.SalesManagerId,
                SaleManagerName = r.SalesManager?.FullName,
                SalesUsers = new List<SaleUsersDto>(r.SalesUsers.Select(u => new SaleUsersDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    IsActive = u.IsActive
                }))
            });
        }

        public async Task<SalesRegionDetailDto?> GetRegionByIdAsync(int id)
        {
            var region = await _context.SalesRegions
                .Include(r => r.SalesManager)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (region == null)
                return null;

            // Get statistics
            var totalSalesUsers = await _context.Users
                .Where(u => u.RegionId == id)
                .CountAsync();

            var activeSalesUsers = await _context.Users
                .Where(u => u.RegionId == id && u.IsActive)
                .CountAsync();

            var totalCustomers = await _context.Customers
                .Where(c => c.RegionId == id)
                .CountAsync();

            var activeCustomers = await _context.Customers
                .Where(c => c.RegionId == id && c.IsActive)
                .CountAsync();

            return new SalesRegionDetailDto
            {
                Id = region.Id,
                Name = region.Name,
                Code = region.Code,
                Description = region.Description,
                IsActive = region.IsActive,
                CreatedDate = region.CreatedDate,
                SaleManagerId = region.SalesManagerId,
                SaleManagerName = region.SalesManager?.FullName,
                TotalSalesUsers = totalSalesUsers,
                ActiveSalesUsers = activeSalesUsers,
                TotalCustomers = totalCustomers,
                ActiveCustomers = activeCustomers
            };
        }

        public async Task<(bool Success, string Message, SalesRegionDto? Region)> CreateRegionAsync(CreateSalesRegionDto dto)
        {
            // Validate code uniqueness
            if (await _regionRepository.CodeExistsAsync(dto.Code))
            {
                return (false, "Region code already exists", null);
            }

            // Validate SalesManager exists if provided
            if (!string.IsNullOrEmpty(dto.SalesManagerId))
            {
                var managerExists = await _context.Users.AnyAsync(u => u.Id == dto.SalesManagerId && u.IsActive);
                if (!managerExists)
                {
                    return (false, "Sales Manager not found or inactive", null);
                }
            }

            var region = new SalesRegion
            {
                Name = dto.Name,
                Code = dto.Code.ToUpper(),
                Description = dto.Description,
                SalesManagerId = dto.SalesManagerId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            await _regionRepository.AddAsync(region);
            await _regionRepository.SaveChangesAsync();

            // Load the manager info for response
            var manager = !string.IsNullOrEmpty(region.SalesManagerId)
                ? await _context.Users.FirstOrDefaultAsync(u => u.Id == region.SalesManagerId)
                : null;

            var regionDto = new SalesRegionDto
            {
                Id = region.Id,
                Name = region.Name,
                Code = region.Code,
                Description = region.Description,
                IsActive = region.IsActive,
                CreatedDate = region.CreatedDate,
                SaleManagerId = region.SalesManagerId,
                SaleManagerName = manager?.FullName
            };

            return (true, "Region created successfully", regionDto);
        }

        public async Task<(bool Success, string Message, SalesRegionDto? Region)> UpdateRegionAsync(int id, UpdateSalesRegionDto dto)
        {
            var region = await _regionRepository.GetByIdAsync(id);
            if (region == null)
            {
                return (false, "Region not found", null);
            }

            // Validate SalesManager exists if provided
            if (!string.IsNullOrEmpty(dto.SalesManagerId))
            {
                var managerExists = await _context.Users.AnyAsync(u => u.Id == dto.SalesManagerId && u.IsActive);
                if (!managerExists)
                {
                    return (false, "Sales Manager not found or inactive", null);
                }
            }

            region.Name = dto.Name;
            region.Description = dto.Description;
            region.IsActive = dto.IsActive;
            region.SalesManagerId = dto.SalesManagerId;
            region.UpdatedDate = DateTime.UtcNow;

            _regionRepository.Update(region);
            await _regionRepository.SaveChangesAsync();

            // Load the manager info for response
            var manager = !string.IsNullOrEmpty(region.SalesManagerId)
                ? await _context.Users.FirstOrDefaultAsync(u => u.Id == region.SalesManagerId)
                : null;

            var regionDto = new SalesRegionDto
            {
                Id = region.Id,
                Name = region.Name,
                Code = region.Code,
                Description = region.Description,
                IsActive = region.IsActive,
                CreatedDate = region.CreatedDate,
                SaleManagerId = region.SalesManagerId,
                SaleManagerName = manager?.FullName
            };

            return (true, "Region updated successfully", regionDto);
        }

        public async Task<(bool Success, string Message)> DeleteRegionAsync(int id)
        {
            var region = await _regionRepository.GetByIdAsync(id);
            if (region == null)
            {
                return (false, "Region not found");
            }

            // Check if region has active users
            var hasActiveUsers = await _context.Users
                .AnyAsync(u => u.RegionId == id && u.IsActive);

            if (hasActiveUsers)
            {
                return (false, "Cannot delete region with active users. Please reassign users first.");
            }

            // Check if region has active customers
            var hasActiveCustomers = await _context.Customers
                .AnyAsync(c => c.RegionId == id && c.IsActive);

            if (hasActiveCustomers)
            {
                return (false, "Cannot delete region with active customers. Please reassign customers first.");
            }

            // Soft delete
            region.IsActive = false;
            region.UpdatedDate = DateTime.UtcNow;

            _regionRepository.Update(region);
            await _regionRepository.SaveChangesAsync();

            return (true, "Region deleted successfully");
        }
    }
}
