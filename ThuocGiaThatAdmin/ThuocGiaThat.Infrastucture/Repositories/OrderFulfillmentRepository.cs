using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public class OrderFulfillmentRepository : IOrderFulfillmentRepository
    {
        private readonly TrueMecContext _context;

        public OrderFulfillmentRepository(TrueMecContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetPendingOrdersAsync(List<int>? orderIds = null)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .Where(o => o.OrderStatus == "Confirmed" || o.OrderStatus == "Pending");

            // Nếu có orderIds được chỉ định, chỉ lấy các đơn đó
            if (orderIds != null && orderIds.Any())
            {
                query = query.Where(o => orderIds.Contains(o.Id));
            }

            // Lọc các đơn hàng có ít nhất 1 OrderItem chưa fulfill hoàn toàn
            var orders = await query
                .Where(o => o.OrderItems.Any(oi => oi.QuantityFulfilled < oi.Quantity))
                .OrderBy(o => o.CreatedDate) // Ưu tiên đơn đặt trước
                .ToListAsync();

            return orders;
        }

        public async Task<List<InventoryBatch>> GetAvailableBatchesAsync(int productVariantId, int warehouseId)
        {
            var batches = await _context.InventoryBatches
                .Include(b => b.Inventory)
                .Where(b => b.Inventory.ProductVariantId == productVariantId
                         && b.Inventory.WarehouseId == warehouseId
                         && (b.Status == BatchStatus.Active || b.Status == BatchStatus.NearExpiry)
                         && b.Quantity - b.QuantitySold > 0)
                .OrderBy(b => b.ExpiryDate) // FEFO: Ưu tiên lô hết hạn sớm nhất
                .ThenBy(b => b.CreatedDate) // Nếu cùng ExpiryDate, ưu tiên lô cũ hơn
                .ToListAsync();

            return batches;
        }

        public async Task<Inventory?> GetInventoryAsync(int productVariantId, int warehouseId)
        {
            return await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductVariantId == productVariantId 
                                       && i.WarehouseId == warehouseId);
        }

        public async Task AddFulfillmentAsync(OrderItemFulfillment fulfillment)
        {
            await _context.OrderItemFulfillments.AddAsync(fulfillment);
        }

        public async Task<Order?> GetOrderFulfillmentDetailsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Fulfillments)
                        .ThenInclude(f => f.InventoryBatch)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<List<BatchLocationStock>> GetBatchLocationsAsync(int inventoryBatchId, int warehouseId)
        {
            return await _context.BatchLocationStocks
                .Where(bl => bl.InventoryBatchId == inventoryBatchId 
                          && bl.WarehouseId == warehouseId
                          && bl.Quantity > 0)
                .OrderByDescending(bl => bl.IsPrimaryLocation)
                .ThenByDescending(bl => bl.Quantity)
                .ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
