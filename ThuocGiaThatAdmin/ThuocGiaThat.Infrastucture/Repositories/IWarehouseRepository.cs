using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public interface IWarehouseRepository : IRepository<Warehouse>
    {
        Task<Warehouse?> GetByCodeAsync(string code);
        Task<IEnumerable<Warehouse>> GetActiveWarehousesAsync();
        Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);
    }
}
