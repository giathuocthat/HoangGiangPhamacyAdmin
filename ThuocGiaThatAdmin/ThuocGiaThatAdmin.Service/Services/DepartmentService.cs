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
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly TrueMecContext _context;

        public DepartmentService(IDepartmentRepository departmentRepository, TrueMecContext context)
        {
            _departmentRepository = departmentRepository;
            _context = context;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync()
        {
            var departments = await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Users)
                .Where(d => d.IsActive)
                .ToListAsync();

            return departments.Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                Description = d.Description,
                IsActive = d.IsActive,
                CreatedDate = d.CreatedDate,
                ManagerId = d.ManagerId,
                ManagerName = d.Manager?.FullName,
                TotalUsers = d.Users.Count
            });
        }

        public async Task<(IEnumerable<DepartmentDto> Departments, int TotalCount)> GetPagedDepartmentsAsync(
            int pageNumber,
            int pageSize,
            string? searchText = null)
        {
            var (departments, totalCount) = await _departmentRepository.GetPagedAsync(pageNumber, pageSize, searchText);

            // Load related data
            var departmentIds = departments.Select(d => d.Id).ToList();
            var departmentsWithData = await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Users)
                .Where(d => departmentIds.Contains(d.Id))
                .ToListAsync();

            var departmentDtos = departmentsWithData.Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                Description = d.Description,
                IsActive = d.IsActive,
                CreatedDate = d.CreatedDate,
                ManagerId = d.ManagerId,
                ManagerName = d.Manager?.FullName,
                TotalUsers = d.Users.Count
            });

            return (departmentDtos, totalCount);
        }

        public async Task<DepartmentDetailDto?> GetDepartmentByIdAsync(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Users.Where(u => u.IsActive))
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
                return null;

            // Get statistics
            var totalUsers = await _context.Users
                .Where(u => u.DepartmentId == id)
                .CountAsync();

            var activeUsers = await _context.Users
                .Where(u => u.DepartmentId == id && u.IsActive)
                .CountAsync();

            return new DepartmentDetailDto
            {
                Id = department.Id,
                Name = department.Name,
                Code = department.Code,
                Description = department.Description,
                IsActive = department.IsActive,
                CreatedDate = department.CreatedDate,
                ManagerId = department.ManagerId,
                ManagerName = department.Manager?.FullName,
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                Users = department.Users.Select(u => new DepartmentUserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    IsActive = u.IsActive
                }).ToList()
            };
        }

        public async Task<(bool Success, string Message, DepartmentDto? Department)> CreateDepartmentAsync(CreateDepartmentDto dto)
        {
            // Validate code uniqueness
            if (await _departmentRepository.CodeExistsAsync(dto.Code))
            {
                return (false, "Department code already exists", null);
            }

            // Validate Manager exists if provided
            if (!string.IsNullOrEmpty(dto.ManagerId))
            {
                var managerExists = await _context.Users.AnyAsync(u => u.Id == dto.ManagerId && u.IsActive);
                if (!managerExists)
                {
                    return (false, "Manager not found or inactive", null);
                }
            }

            var department = new Department
            {
                Name = dto.Name,
                Code = dto.Code.ToUpper(),
                Description = dto.Description,
                ManagerId = dto.ManagerId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            await _departmentRepository.AddAsync(department);
            await _departmentRepository.SaveChangesAsync();

            // Load the manager info for response
            var manager = !string.IsNullOrEmpty(department.ManagerId)
                ? await _context.Users.FirstOrDefaultAsync(u => u.Id == department.ManagerId)
                : null;

            var departmentDto = new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Code = department.Code,
                Description = department.Description,
                IsActive = department.IsActive,
                CreatedDate = department.CreatedDate,
                ManagerId = department.ManagerId,
                ManagerName = manager?.FullName,
                TotalUsers = 0
            };

            return (true, "Department created successfully", departmentDto);
        }

        public async Task<(bool Success, string Message, DepartmentDto? Department)> UpdateDepartmentAsync(int id, UpdateDepartmentDto dto)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
            {
                return (false, "Department not found", null);
            }

            // Validate Manager exists if provided
            if (!string.IsNullOrEmpty(dto.ManagerId))
            {
                var managerExists = await _context.Users.AnyAsync(u => u.Id == dto.ManagerId && u.IsActive);
                if (!managerExists)
                {
                    return (false, "Manager not found or inactive", null);
                }
            }

            department.Name = dto.Name;
            department.Description = dto.Description;
            department.IsActive = dto.IsActive;
            department.ManagerId = dto.ManagerId;
            department.UpdatedDate = DateTime.UtcNow;

            _departmentRepository.Update(department);
            await _departmentRepository.SaveChangesAsync();

            // Load the manager info for response
            var manager = !string.IsNullOrEmpty(department.ManagerId)
                ? await _context.Users.FirstOrDefaultAsync(u => u.Id == department.ManagerId)
                : null;

            var userCount = await _context.Users.CountAsync(u => u.DepartmentId == id);

            var departmentDto = new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Code = department.Code,
                Description = department.Description,
                IsActive = department.IsActive,
                CreatedDate = department.CreatedDate,
                ManagerId = department.ManagerId,
                ManagerName = manager?.FullName,
                TotalUsers = userCount
            };

            return (true, "Department updated successfully", departmentDto);
        }

        public async Task<(bool Success, string Message)> DeleteDepartmentAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
            {
                return (false, "Department not found");
            }

            // Check if department has active users
            var hasActiveUsers = await _context.Users
                .AnyAsync(u => u.DepartmentId == id && u.IsActive);

            if (hasActiveUsers)
            {
                return (false, "Cannot delete department with active users. Please reassign users first.");
            }

            // Soft delete
            department.IsActive = false;
            department.UpdatedDate = DateTime.UtcNow;

            _departmentRepository.Update(department);
            await _departmentRepository.SaveChangesAsync();

            return (true, "Department deleted successfully");
        }
    }
}
