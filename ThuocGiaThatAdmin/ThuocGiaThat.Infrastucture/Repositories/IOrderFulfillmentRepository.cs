using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public interface IOrderFulfillmentRepository
    {
        /// <summary>
        /// Lấy danh sách đơn hàng cần fulfill
        /// </summary>
        /// <param name="orderIds">Danh sách OrderId cụ thể (optional)</param>
        /// <returns>Danh sách Order với OrderItems chưa fulfill hoàn toàn</returns>
        Task<List<Order>> GetPendingOrdersAsync(List<int>? orderIds = null);
        
        /// <summary>
        /// Lấy danh sách InventoryBatch available cho một ProductVariant trong một Warehouse
        /// Sắp xếp theo ExpiryDate tăng dần (FEFO - First Expiry First Out)
        /// </summary>
        /// <param name="productVariantId">ID của ProductVariant</param>
        /// <param name="warehouseId">ID của Warehouse</param>
        /// <returns>Danh sách InventoryBatch available</returns>
        Task<List<InventoryBatch>> GetAvailableBatchesAsync(int productVariantId, int warehouseId);
        
        /// <summary>
        /// Lấy Inventory record cho ProductVariant và Warehouse
        /// </summary>
        Task<Inventory?> GetInventoryAsync(int productVariantId, int warehouseId);
        
        /// <summary>
        /// Thêm OrderItemFulfillment record
        /// </summary>
        Task AddFulfillmentAsync(OrderItemFulfillment fulfillment);
        
        /// <summary>
        /// Lưu thay đổi vào database
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
