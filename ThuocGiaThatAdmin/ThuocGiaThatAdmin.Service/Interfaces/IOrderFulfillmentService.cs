using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface IOrderFulfillmentService
    {
        /// <summary>
        /// Fulfill đơn hàng - Book sản phẩm từ kho cho đơn hàng
        /// </summary>
        /// <param name="request">Request chứa OrderIds và WarehouseId</param>
        /// <param name="userId">ID của user thực hiện fulfill</param>
        /// <returns>Response chứa kết quả fulfill</returns>
        Task<FulfillOrderResponseDto> FulfillOrdersAsync(FulfillOrderRequestDto request, Guid userId);
    }
}
