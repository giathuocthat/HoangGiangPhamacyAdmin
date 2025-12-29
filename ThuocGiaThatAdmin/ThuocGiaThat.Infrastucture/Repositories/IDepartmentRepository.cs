using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        /// <summary>
        /// Lấy tất cả departments đang active
        /// </summary>
        Task<IEnumerable<Department>> GetAllActiveAsync();

        /// <summary>
        /// Lấy tất cả departments với phân trang
        /// </summary>
        Task<(IEnumerable<Department> Departments, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchText = null);

        /// <summary>
        /// Lấy department theo code
        /// </summary>
        Task<Department?> GetByCodeAsync(string code);

        /// <summary>
        /// Lấy department với danh sách users
        /// </summary>
        Task<Department?> GetDepartmentWithUsersAsync(int id);

        /// <summary>
        /// Lấy department với manager
        /// </summary>
        Task<Department?> GetDepartmentWithManagerAsync(int id);


        /// <summary>
        /// Kiểm tra code đã tồn tại chưa
        /// </summary>
        Task<bool> CodeExistsAsync(string code, int? excludeId = null);
    }
}
