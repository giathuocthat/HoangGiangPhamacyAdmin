using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThat.Infrastucture.Repositories;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class StockAlertService
    {
        private readonly IStockAlertRepository _alertRepository;
        private readonly IRepository<ProductVariant> _productVariantRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IInventoryBatchRepository _batchRepository;

        public StockAlertService(
            IStockAlertRepository alertRepository,
            IRepository<ProductVariant> productVariantRepository,
            IWarehouseRepository warehouseRepository,
            IInventoryBatchRepository batchRepository)
        {
            _alertRepository = alertRepository;
            _productVariantRepository = productVariantRepository;
            _warehouseRepository = warehouseRepository;
            _batchRepository = batchRepository;
        }

        public async Task<IEnumerable<StockAlertDto>> GetUnreadAlertsAsync()
        {
            var alerts = await _alertRepository.GetUnreadAlertsAsync();
            return await MapToDtosAsync(alerts);
        }

        public async Task<IEnumerable<StockAlertDto>> GetUnresolvedAlertsAsync()
        {
            var alerts = await _alertRepository.GetUnresolvedAlertsAsync();
            return await MapToDtosAsync(alerts);
        }

        public async Task<IEnumerable<StockAlertDto>> GetAlertsByTypeAsync(AlertType type)
        {
            var alerts = await _alertRepository.GetByTypeAsync(type);
            return await MapToDtosAsync(alerts);
        }

        public async Task<IEnumerable<StockAlertDto>> GetAlertsByPriorityAsync(AlertPriority priority)
        {
            var alerts = await _alertRepository.GetByPriorityAsync(priority);
            return await MapToDtosAsync(alerts);
        }

        public async Task MarkAsReadAsync(int id)
        {
            var alert = await _alertRepository.GetByIdAsync(id);
            if (alert == null)
            {
                throw new KeyNotFoundException($"Alert with ID {id} not found");
            }

            alert.IsRead = true;
            _alertRepository.Update(alert);
            await _alertRepository.SaveChangesAsync();
        }

        public async Task ResolveAlertAsync(int id, ResolveAlertDto dto, string? userId = null)
        {
            var alert = await _alertRepository.GetByIdAsync(id);
            if (alert == null)
            {
                throw new KeyNotFoundException($"Alert with ID {id} not found");
            }

            alert.IsResolved = true;
            alert.ResolvedDate = DateTime.Now;
            alert.ResolvedByUserId = userId;
            alert.ResolutionNotes = dto.ResolutionNotes;

            _alertRepository.Update(alert);
            await _alertRepository.SaveChangesAsync();
        }

        private async Task<List<StockAlertDto>> MapToDtosAsync(IEnumerable<StockAlert> alerts)
        {
            var result = new List<StockAlertDto>();

            foreach (var alert in alerts)
            {
                var variant = await _productVariantRepository.GetByIdAsync(alert.ProductVariantId);
                var warehouse = await _warehouseRepository.GetByIdAsync(alert.WarehouseId);
                InventoryBatch? batch = null;

                if (alert.BatchId.HasValue)
                {
                    batch = await _batchRepository.GetByIdAsync(alert.BatchId.Value);
                }

                if (variant != null && warehouse != null)
                {
                    result.Add(new StockAlertDto
                    {
                        Id = alert.Id,
                        ProductVariantId = alert.ProductVariantId,
                        ProductName = variant.Product?.Name ?? "Unknown",
                        SKU = variant.SKU,
                        WarehouseId = alert.WarehouseId,
                        WarehouseName = warehouse.Name,
                        BatchId = alert.BatchId,
                        BatchNumber = batch?.BatchNumber,
                        Type = alert.Type,
                        Priority = alert.Priority,
                        Message = alert.Message,
                        CurrentQuantity = alert.CurrentQuantity,
                        ExpiryDate = alert.ExpiryDate,
                        IsRead = alert.IsRead,
                        IsResolved = alert.IsResolved,
                        ResolvedDate = alert.ResolvedDate,
                        ResolutionNotes = alert.ResolutionNotes,
                        CreatedDate = alert.CreatedDate
                    });
                }
            }

            return result;
        }
    }
}
