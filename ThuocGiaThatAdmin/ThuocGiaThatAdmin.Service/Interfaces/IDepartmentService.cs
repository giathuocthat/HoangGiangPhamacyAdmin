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
    }
}
