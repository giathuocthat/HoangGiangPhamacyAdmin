using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Domain.Constants;
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
            var regions = await _regionRepository.GetAllActiveAsync();

            // Get all region IDs
            var regionIds = regions.Select(r => r.Id).ToList();

            // Get Sale Managers for all regions (users with "Sale Manager" role in each region)
            var saleManagers = await (from u in _context.Users
                                      join ur in _context.UserRoles on u.Id equals ur.UserId
                                      join r in _context.Roles on ur.RoleId equals r.Id
                                      where u.RegionId.HasValue
                                        && regionIds.Contains(u.RegionId.Value)
                                        && r.Name == SaleManagerPermission.Role
                                        && u.IsActive
                                      select new { u.Id, u.FullName, u.RegionId })
                                      .ToListAsync();

            return regions.Select(r =>
            {
                var manager = saleManagers.FirstOrDefault(m => m.RegionId == r.Id);
                return new SalesRegionDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Code = r.Code,
                    Description = r.Description,
                    IsActive = r.IsActive,
                    CreatedDate = r.CreatedDate,
                    SaleManagerId = manager?.Id,
                    SaleManagerName = manager?.FullName
                };
            });
        }

        public async Task<SalesRegionDetailDto?> GetRegionByIdAsync(int id)
        {
            var region = await _regionRepository.GetByIdAsync(id);
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

            var region = new SalesRegion
            {
                Name = dto.Name,
                Code = dto.Code.ToUpper(),
                Description = dto.Description,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            await _regionRepository.AddAsync(region);
            await _regionRepository.SaveChangesAsync();

            var regionDto = new SalesRegionDto
            {
                Id = region.Id,
                Name = region.Name,
                Code = region.Code,
                Description = region.Description,
                IsActive = region.IsActive,
                CreatedDate = region.CreatedDate
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

            region.Name = dto.Name;
            region.Description = dto.Description;
            region.IsActive = dto.IsActive;
            region.UpdatedDate = DateTime.UtcNow;

            _regionRepository.Update(region);
            await _regionRepository.SaveChangesAsync();

            var regionDto = new SalesRegionDto
            {
                Id = region.Id,
                Name = region.Name,
                Code = region.Code,
                Description = region.Description,
                IsActive = region.IsActive,
                CreatedDate = region.CreatedDate
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
