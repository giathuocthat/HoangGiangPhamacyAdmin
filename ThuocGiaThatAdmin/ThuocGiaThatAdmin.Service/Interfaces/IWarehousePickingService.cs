using System;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    /// <summary>
    /// Service interface cho warehouse picking operations
    /// </summary>
    public interface IWarehousePickingService
    {
        /// <summary>
        /// Xử lý batch picking movements - chuyển hàng từ nhiều vị trí sang thùng hàng
        /// </summary>
        /// <param name="request">Request chứa danh sách movements</param>
        /// <param name="userId">ID của user thực hiện</param>
        /// <returns>Kết quả xử lý với danh sách successful/failed movements</returns>
        Task<ProcessPickingResponseDto> ProcessPickingAsync(
            ProcessPickingRequestDto request, 
            Guid userId);
        
        /// <summary>
        /// Validate location code và batch number
        /// </summary>
        /// <param name="locationCode">Mã vị trí</param>
        /// <param name="batchNumber">Số lô</param>
        /// <param name="warehouseId">ID kho</param>
        /// <returns>Thông tin validation kèm product details</returns>
        Task<ValidatePickingResponseDto> ValidatePickingAsync(
            string locationCode, 
            string batchNumber, 
            int warehouseId);
    }
}
