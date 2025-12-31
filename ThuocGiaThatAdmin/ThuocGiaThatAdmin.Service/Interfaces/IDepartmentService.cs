using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface IDepartmentService
    {
        /// <summary>
        /// Lấy tất cả departments đang active
        /// </summary>
        Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync();

        /// <summary>
        /// Lấy departments với phân trang
        /// </summary>
        Task<(IEnumerable<DepartmentDto> Departments, int TotalCount)> GetPagedDepartmentsAsync(
            int pageNumber,
            int pageSize,
            string? searchText = null);

        /// <summary>
        /// Lấy department theo ID với thống kê
        /// </summary>
        Task<DepartmentDetailDto?> GetDepartmentByIdAsync(int id);

        /// <summary>
        /// Tạo department mới
        /// </summary>
        Task<(bool Success, string Message, DepartmentDto? Department)> CreateDepartmentAsync(CreateDepartmentDto dto);

        /// <summary>
        /// Cập nhật department
        /// </summary>
        Task<(bool Success, string Message, DepartmentDto? Department)> UpdateDepartmentAsync(int id, UpdateDepartmentDto dto);

        /// <summary>
        /// Xóa department (soft delete)
        /// </summary>
        Task<(bool Success, string Message)> DeleteDepartmentAsync(int id);

        // ========== Department Role Management ==========

        /// <summary>
        /// Lấy danh sách roles của department
        /// </summary>
        Task<IEnumerable<DepartmentRoleDto>> GetDepartmentRolesAsync(int departmentId);

        /// <summary>
        /// Assign role cho department
        /// </summary>
        Task<(bool Success, string Message)> AssignRoleToDepartmentAsync(int departmentId, AssignRoleToDepartmentDto dto);

        /// <summary>
        /// Remove role khỏi department
        /// </summary>
        Task<(bool Success, string Message)> RemoveRoleFromDepartmentAsync(int departmentId, string roleId);

        /// <summary>
        /// Lấy danh sách roles chưa được assign cho department
        /// </summary>
        Task<IEnumerable<RoleDto>> GetAvailableRolesForDepartmentAsync(int departmentId);
    }
}
