using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public interface IStockAlertRepository : IRepository<StockAlert>
    {
        Task<IEnumerable<StockAlert>> GetUnreadAlertsAsync();
        Task<IEnumerable<StockAlert>> GetUnresolvedAlertsAsync();
        Task<IEnumerable<StockAlert>> GetByTypeAsync(AlertType type);
        Task<IEnumerable<StockAlert>> GetByPriorityAsync(AlertPriority priority);
    }
}
