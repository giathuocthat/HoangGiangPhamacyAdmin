using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository implementation for PurchaseOrder entity
    /// </summary>
    public class PurchaseOrderRepository : Repository<PurchaseOrder>, IPurchaseOrderRepository
    {
        public PurchaseOrderRepository(TrueMecContext context) : base(context)
        {
        }

        public async Task<PurchaseOrder?> GetByOrderNumberAsync(string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
                throw new ArgumentException("Order number cannot be null or empty", nameof(orderNumber));

            return await _dbSet
                .FirstOrDefaultAsync(po => po.OrderNumber == orderNumber);
        }

        public async Task<PurchaseOrder?> GetWithDetailsAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Purchase Order ID must be greater than 0", nameof(id));

            return await _dbSet
                .Include(po => po.Supplier)
                .Include(po => po.SupplierContact)
                .Include(po => po.Warehouse)
                .Include(po => po.PurchaseOrderItems)
                    .ThenInclude(poi => poi.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .Include(po => po.GoodsReceipts)
                    .ThenInclude(gr => gr.GoodsReceiptItems)
                .Include(po => po.PurchaseOrderHistories.OrderByDescending(h => h.ChangedDate))
                .FirstOrDefaultAsync(po => po.Id == id);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetBySupplierIdAsync(int supplierId)
        {
            if (supplierId <= 0)
                throw new ArgumentException("Supplier ID must be greater than 0", nameof(supplierId));

            return await _dbSet
                .Where(po => po.SupplierId == supplierId)
                .Include(po => po.Warehouse)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseOrder>> GetByWarehouseIdAsync(int warehouseId)
        {
            if (warehouseId <= 0)
                throw new ArgumentException("Warehouse ID must be greater than 0", nameof(warehouseId));

            return await _dbSet
                .Where(po => po.WarehouseId == warehouseId)
                .Include(po => po.Supplier)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseOrder>> GetByStatusAsync(PurchaseOrderStatus status)
        {
            return await _dbSet
                .Where(po => po.Status == status)
                .Include(po => po.Supplier)
                .Include(po => po.Warehouse)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();
        }

        public async Task<(IList<PurchaseOrder> orders, int totalCount)> GetPagedPurchaseOrdersAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            int? supplierId = null,
            int? warehouseId = null,
            PurchaseOrderStatus? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var query = _dbSet.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(po => po.OrderNumber.Contains(searchTerm));
            }

            if (supplierId.HasValue)
            {
                query = query.Where(po => po.SupplierId == supplierId.Value);
            }

            if (warehouseId.HasValue)
            {
                query = query.Where(po => po.WarehouseId == warehouseId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(po => po.Status == status.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(po => po.OrderDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(po => po.OrderDate <= toDate.Value);
            }

            var totalCount = await query.CountAsync();

            var orders = await query
                .Include(po => po.Supplier)
                .Include(po => po.Warehouse)
                .Include(po => po.PurchaseOrderItems)
                .OrderByDescending(po => po.OrderDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (orders, totalCount);
        }

        public async Task<string> GenerateOrderNumberAsync()
        {
            var today = DateTime.UtcNow;
            var prefix = $"PO-{today:yyyyMMdd}";

            var lastOrder = await _dbSet
                .Where(po => po.OrderNumber.StartsWith(prefix))
                .OrderByDescending(po => po.OrderNumber)
                .FirstOrDefaultAsync();

            if (lastOrder == null)
            {
                return $"{prefix}-0001";
            }

            var lastNumber = int.Parse(lastOrder.OrderNumber.Substring(prefix.Length + 1));
            var nextNumber = lastNumber + 1;

            return $"{prefix}-{nextNumber:D4}";
        }

        public async Task<bool> OrderNumberExistsAsync(string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
                return false;

            return await _dbSet.AnyAsync(po => po.OrderNumber == orderNumber);
        }
    }

    /// <summary>
    /// Repository implementation for PurchaseOrderItem entity
    /// </summary>
    public class PurchaseOrderItemRepository : Repository<PurchaseOrderItem>, IPurchaseOrderItemRepository
    {
        public PurchaseOrderItemRepository(TrueMecContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PurchaseOrderItem>> GetByPurchaseOrderIdAsync(int purchaseOrderId)
        {
            if (purchaseOrderId <= 0)
                throw new ArgumentException("Purchase Order ID must be greater than 0", nameof(purchaseOrderId));

            return await _dbSet
                .Where(poi => poi.PurchaseOrderId == purchaseOrderId)
                .Include(poi => poi.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                .ToListAsync();
        }

        public async Task<PurchaseOrderItem?> GetWithProductVariantAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Purchase Order Item ID must be greater than 0", nameof(id));

            return await _dbSet
                .Include(poi => poi.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                .Include(poi => poi.GoodsReceiptItems)
                .FirstOrDefaultAsync(poi => poi.Id == id);
        }
    }

    /// <summary>
    /// Repository implementation for PurchaseOrderHistory entity
    /// </summary>
    public class PurchaseOrderHistoryRepository : Repository<PurchaseOrderHistory>, IPurchaseOrderHistoryRepository
    {
        public PurchaseOrderHistoryRepository(TrueMecContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PurchaseOrderHistory>> GetByPurchaseOrderIdAsync(int purchaseOrderId)
        {
            if (purchaseOrderId <= 0)
                throw new ArgumentException("Purchase Order ID must be greater than 0", nameof(purchaseOrderId));

            return await _dbSet
                .Where(h => h.PurchaseOrderId == purchaseOrderId)
                .OrderByDescending(h => h.ChangedDate)
                .ToListAsync();
        }

        public async Task<PurchaseOrderHistory?> GetLatestByPurchaseOrderIdAsync(int purchaseOrderId)
        {
            if (purchaseOrderId <= 0)
                throw new ArgumentException("Purchase Order ID must be greater than 0", nameof(purchaseOrderId));

            return await _dbSet
                .Where(h => h.PurchaseOrderId == purchaseOrderId)
                .OrderByDescending(h => h.ChangedDate)
                .FirstOrDefaultAsync();
        }
    }
}
