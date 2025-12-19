using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Utils;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Domain.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    /// <summary>
    /// Repository implementation for GoodsReceipt entity
    /// </summary>
    public class GoodsReceiptRepository : Repository<GoodsReceipt>, IGoodsReceiptRepository
    {
        private readonly DynamicFilterService _filterService;

        public GoodsReceiptRepository(TrueMecContext context, DynamicFilterService filterService) : base(context)
        {
            _filterService = filterService;
        }

        public async Task<GoodsReceipt?> GetByReceiptNumberAsync(string receiptNumber)
        {
            if (string.IsNullOrWhiteSpace(receiptNumber))
                throw new ArgumentException("Receipt number cannot be null or empty", nameof(receiptNumber));

            return await _dbSet
                .FirstOrDefaultAsync(gr => gr.ReceiptNumber == receiptNumber);
        }

        public async Task<GoodsReceipt?> GetWithDetailsAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Goods Receipt ID must be greater than 0", nameof(id));

            return await _dbSet
                .Include(gr => gr.PurchaseOrder)
                    .ThenInclude(po => po.Supplier)
                .Include(gr => gr.Warehouse)
                .Include(gr => gr.GoodsReceiptItems)
                    .ThenInclude(gri => gri.PurchaseOrderItem)
                        .ThenInclude(poi => poi.ProductVariant)
                            .ThenInclude(pv => pv.Product)
                .FirstOrDefaultAsync(gr => gr.Id == id);
        }

        public async Task<IEnumerable<GoodsReceipt>> GetByPurchaseOrderIdAsync(int purchaseOrderId)
        {
            if (purchaseOrderId <= 0)
                throw new ArgumentException("Purchase Order ID must be greater than 0", nameof(purchaseOrderId));

            return await _dbSet
                .Where(gr => gr.PurchaseOrderId == purchaseOrderId)
                .Include(gr => gr.Warehouse)
                .Include(gr => gr.GoodsReceiptItems)
                .OrderByDescending(gr => gr.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<GoodsReceipt>> GetByWarehouseIdAsync(int warehouseId)
        {
            if (warehouseId <= 0)
                throw new ArgumentException("Warehouse ID must be greater than 0", nameof(warehouseId));

            return await _dbSet
                .Where(gr => gr.WarehouseId == warehouseId)
                .Include(gr => gr.PurchaseOrder)
                    .ThenInclude(po => po.Supplier)
                .OrderByDescending(gr => gr.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<GoodsReceipt>> GetByStatusAsync(GoodsReceiptStatus status)
        {
            return await _dbSet
                .Where(gr => gr.Status == status)
                .Include(gr => gr.PurchaseOrder)
                    .ThenInclude(po => po.Supplier)
                .Include(gr => gr.Warehouse)
                .OrderByDescending(gr => gr.ScheduledDate ?? gr.CreatedDate)
                .ToListAsync();
        }

        public async Task<(IList<GoodsReceipt> receipts, int totalCount)> GetFilteredGoodsReceiptsAsync(FilterRequest request)
        {
            var query = _dbSet.AsQueryable();
            query = _filterService.ApplyFilters(query, request);

            var totalCount = await query.CountAsync();

            // Apply pagination with navigation properties
            var items = await query
                .Include(gr => gr.PurchaseOrder)
                    .ThenInclude(po => po.Supplier)
                .Include(gr => gr.Warehouse)
                .Include(gr => gr.GoodsReceiptItems)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(IList<GoodsReceipt> receipts, int totalCount)> GetPagedGoodsReceiptsAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            int? purchaseOrderId = null,
            int? warehouseId = null,
            GoodsReceiptStatus? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var query = _dbSet.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(gr =>
                    gr.ReceiptNumber.Contains(searchTerm) ||
                    gr.PurchaseOrder.OrderNumber.Contains(searchTerm));
            }

            if (purchaseOrderId.HasValue)
            {
                query = query.Where(gr => gr.PurchaseOrderId == purchaseOrderId.Value);
            }

            if (warehouseId.HasValue)
            {
                query = query.Where(gr => gr.WarehouseId == warehouseId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(gr => gr.Status == status.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(gr =>
                    (gr.ScheduledDate.HasValue && gr.ScheduledDate.Value >= fromDate.Value) ||
                    gr.CreatedDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(gr =>
                    (gr.ScheduledDate.HasValue && gr.ScheduledDate.Value <= toDate.Value) ||
                    gr.CreatedDate <= toDate.Value);
            }

            var totalCount = await query.CountAsync();

            var receipts = await query
                .Include(gr => gr.PurchaseOrder)
                    .ThenInclude(po => po.Supplier)
                .Include(gr => gr.Warehouse)
                .Include(gr => gr.GoodsReceiptItems)
                .OrderByDescending(gr => gr.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (receipts, totalCount);
        }

        public async Task<string> GenerateReceiptNumberAsync()
        {
            var today = DateTime.UtcNow;
            var prefix = $"GR-{today:yyyyMMdd}";

            var lastReceipt = await _dbSet
                .Where(gr => gr.ReceiptNumber.StartsWith(prefix))
                .OrderByDescending(gr => gr.ReceiptNumber)
                .FirstOrDefaultAsync();

            if (lastReceipt == null)
            {
                return $"{prefix}-0001";
            }

            var lastNumber = int.Parse(lastReceipt.ReceiptNumber.Substring(prefix.Length + 1));
            var nextNumber = lastNumber + 1;

            return $"{prefix}-{nextNumber:D4}";
        }

        public async Task<bool> ReceiptNumberExistsAsync(string receiptNumber)
        {
            if (string.IsNullOrWhiteSpace(receiptNumber))
                return false;

            return await _dbSet.AnyAsync(gr => gr.ReceiptNumber == receiptNumber);
        }
    }

    /// <summary>
    /// Repository implementation for GoodsReceiptItem entity
    /// </summary>
    public class GoodsReceiptItemRepository : Repository<GoodsReceiptItem>, IGoodsReceiptItemRepository
    {
        public GoodsReceiptItemRepository(TrueMecContext context) : base(context)
        {
        }

        public async Task<IEnumerable<GoodsReceiptItem>> GetByGoodsReceiptIdAsync(int goodsReceiptId)
        {
            if (goodsReceiptId <= 0)
                throw new ArgumentException("Goods Receipt ID must be greater than 0", nameof(goodsReceiptId));

            return await _dbSet
                .Where(gri => gri.GoodsReceiptId == goodsReceiptId)
                .Include(gri => gri.PurchaseOrderItem)
                    .ThenInclude(poi => poi.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .ToListAsync();
        }

        public async Task<IEnumerable<GoodsReceiptItem>> GetByPurchaseOrderItemIdAsync(int purchaseOrderItemId)
        {
            if (purchaseOrderItemId <= 0)
                throw new ArgumentException("Purchase Order Item ID must be greater than 0", nameof(purchaseOrderItemId));

            return await _dbSet
                .Where(gri => gri.PurchaseOrderItemId == purchaseOrderItemId)
                .Include(gri => gri.GoodsReceipt)
                .OrderByDescending(gri => gri.CreatedDate)
                .ToListAsync();
        }

        public async Task<GoodsReceiptItem?> GetWithPurchaseOrderItemAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Goods Receipt Item ID must be greater than 0", nameof(id));

            return await _dbSet
                .Include(gri => gri.PurchaseOrderItem)
                    .ThenInclude(poi => poi.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .Include(gri => gri.GoodsReceipt)
                    .ThenInclude(gr => gr.PurchaseOrder)
                .FirstOrDefaultAsync(gri => gri.Id == id);
        }
    }
}
