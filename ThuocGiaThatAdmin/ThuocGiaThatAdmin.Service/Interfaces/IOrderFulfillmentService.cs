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
        
        /// <summary>
        /// Lấy order fulfillment details cho warehouse picking
        /// Bao gồm thông tin batches đã fulfill và suggested locations
        /// </summary>
        /// <param name="orderIdentifier">ID hoặc OrderNumber của Order (e.g., "123" hoặc "ORD-20251219-357203")</param>
        /// <param name="warehouseId">ID của Warehouse</param>
        /// <returns>Order fulfillment details với suggested locations</returns>
        Task<OrderFulfillmentDetailsResponseDto> GetOrderFulfillmentDetailsAsync(string orderIdentifier, int warehouseId);
    }
}
