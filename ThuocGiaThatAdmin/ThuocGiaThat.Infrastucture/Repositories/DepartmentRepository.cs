using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(TrueMecContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Department>> GetAllActiveAsync()
        {
            return await _context.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Department> Departments, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchText = null)
        {
            var query = _context.Departments.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(d =>
                    d.Name.Contains(searchText) ||
                    d.Code.Contains(searchText) ||
                    (d.Description != null && d.Description.Contains(searchText)));
            }

            var totalCount = await query.CountAsync();

            var departments = await query
                .OrderBy(d => d.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (departments, totalCount);
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Department?> GetByCodeAsync(string code)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(d => d.Code == code);
        }

        public async Task<Department?> GetDepartmentWithUsersAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Users)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Department?> GetDepartmentWithManagerAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Manager)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Department> AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
            return department;
        }

        public void Update(Department department)
        {
            _context.Departments.Update(department);
        }

        public void Delete(Department department)
        {
            _context.Departments.Remove(department);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
        {
            var query = _context.Departments.Where(d => d.Code == code);

            if (excludeId.HasValue)
            {
                query = query.Where(d => d.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
