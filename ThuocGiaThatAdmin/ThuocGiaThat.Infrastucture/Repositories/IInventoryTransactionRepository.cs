using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public interface IInventoryTransactionRepository : IRepository<InventoryTransaction>
    {
        Task<IEnumerable<InventoryTransaction>> GetByProductVariantIdAsync(int productVariantId);
        Task<IEnumerable<InventoryTransaction>> GetByWarehouseIdAsync(int warehouseId);
        Task<IEnumerable<InventoryTransaction>> GetByTypeAsync(TransactionType type);
        Task<IEnumerable<InventoryTransaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
