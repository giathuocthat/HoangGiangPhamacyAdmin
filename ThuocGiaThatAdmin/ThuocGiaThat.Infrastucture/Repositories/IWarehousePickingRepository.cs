using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository interface cho warehouse picking operations
    /// </summary>
    public interface IWarehousePickingRepository
    {
        /// <summary>
        /// Lấy BatchLocationStock theo location code, batch number và warehouse
        /// </summary>
        Task<BatchLocationStock?> GetBatchLocationAsync(
            string locationCode, 
            string batchNumber, 
            int warehouseId);
        
        /// <summary>
        /// Thêm LocationStockMovement record
        /// </summary>
        Task AddMovementAsync(LocationStockMovement movement);
        
        /// <summary>
        /// Cập nhật BatchLocationStock
        /// </summary>
        Task UpdateBatchLocationStockAsync(BatchLocationStock stock);
        
        /// <summary>
        /// Lấy hoặc tạo mới BatchLocationStock tại vị trí đích
        /// </summary>
        Task<BatchLocationStock> GetOrCreateDestinationStockAsync(
            int inventoryBatchId,
            int productVariantId,
            int warehouseId,
            string destinationLocationCode);
        
        /// <summary>
        /// Lưu thay đổi vào database
        /// </summary>
        Task SaveChangesAsync();
    }
}
