using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface ISalesRegionService
    {
        /// <summary>
        /// Lấy tất cả regions đang active
        /// </summary>
        Task<IEnumerable<SalesRegionDto>> GetAllRegionsAsync();

        /// <summary>
        /// Lấy region theo ID với thống kê
        /// </summary>
        Task<SalesRegionDetailDto?> GetRegionByIdAsync(int id);

        /// <summary>
        /// Tạo region mới
        /// </summary>
        Task<(bool Success, string Message, SalesRegionDto? Region)> CreateRegionAsync(CreateSalesRegionDto dto);

        /// <summary>
        /// Cập nhật region
        /// </summary>
        Task<(bool Success, string Message, SalesRegionDto? Region)> UpdateRegionAsync(int id, UpdateSalesRegionDto dto);

        /// <summary>
        /// Xóa region (soft delete)
        /// </summary>
        Task<(bool Success, string Message)> DeleteRegionAsync(int id);
    }
}
