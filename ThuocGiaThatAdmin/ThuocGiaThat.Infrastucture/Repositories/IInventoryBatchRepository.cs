using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public interface IInventoryBatchRepository : IRepository<InventoryBatch>
    {
        Task<IEnumerable<InventoryBatch>> GetByInventoryIdAsync(int inventoryId);
        Task<IEnumerable<InventoryBatch>> GetNearExpiryBatchesAsync(int months = 3);
        Task<IEnumerable<InventoryBatch>> GetExpiredBatchesAsync();
        Task<InventoryBatch?> GetOldestActiveBatchAsync(int inventoryId);
    }
}
