using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public interface ISalesRegionRepository
    {
        /// <summary>
        /// Lấy tất cả regions đang active
        /// </summary>
        Task<IEnumerable<SalesRegion>> GetAllActiveAsync();

        /// <summary>
        /// Lấy region theo ID
        /// </summary>
        Task<SalesRegion?> GetByIdAsync(int id);

        /// <summary>
        /// Lấy region theo code
        /// </summary>
        Task<SalesRegion?> GetByCodeAsync(string code);

        /// <summary>
        /// Lấy region với danh sách users
        /// </summary>
        Task<SalesRegion?> GetRegionWithUsersAsync(int id);

        /// <summary>
        /// Lấy region với danh sách customers
        /// </summary>
        Task<SalesRegion?> GetRegionWithCustomersAsync(int id);

        /// <summary>
        /// Thêm region mới
        /// </summary>
        Task<SalesRegion> AddAsync(SalesRegion region);

        /// <summary>
        /// Cập nhật region
        /// </summary>
        void Update(SalesRegion region);

        /// <summary>
        /// Xóa region
        /// </summary>
        void Delete(SalesRegion region);

        /// <summary>
        /// Lưu thay đổi
        /// </summary>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Kiểm tra code đã tồn tại chưa
        /// </summary>
        Task<bool> CodeExistsAsync(string code, int? excludeId = null);
    }
}
