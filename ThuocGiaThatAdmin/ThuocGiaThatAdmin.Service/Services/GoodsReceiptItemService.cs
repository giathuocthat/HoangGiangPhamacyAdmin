using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Contracts.Responses;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Domain.Enums;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service.Services
{
    /// <summary>
    /// Service implementation for GoodsReceiptItem business logic
    /// </summary>
    public class GoodsReceiptItemService : IGoodsReceiptItemService
    {
        private readonly IGoodsReceiptItemRepository _itemRepository;
        private readonly IGoodsReceiptRepository _goodsReceiptRepository;
        private readonly IPurchaseOrderItemRepository _purchaseOrderItemRepository;

        public GoodsReceiptItemService(
            IGoodsReceiptItemRepository itemRepository,
            IGoodsReceiptRepository goodsReceiptRepository,
            IPurchaseOrderItemRepository purchaseOrderItemRepository)
        {
            _itemRepository = itemRepository ?? throw new ArgumentNullException(nameof(itemRepository));
            _goodsReceiptRepository = goodsReceiptRepository ?? throw new ArgumentNullException(nameof(goodsReceiptRepository));
            _purchaseOrderItemRepository = purchaseOrderItemRepository ?? throw new ArgumentNullException(nameof(purchaseOrderItemRepository));
        }

        #region CRUD Operations

        public async Task<IEnumerable<GoodsReceiptItemResponse>> GetAllAsync()
        {
            var items = await _itemRepository.GetAllAsync();
            return items.Select(MapToResponse);
        }

        public async Task<GoodsReceiptItemResponse?> GetByIdAsync(int id)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            return item != null ? MapToResponse(item) : null;
        }

        public async Task<GoodsReceiptItemResponse> CreateAsync(int goodsReceiptId, CreateGoodsReceiptItemDto dto)
        {
            // Validate goods receipt exists
            var goodsReceipt = await _goodsReceiptRepository.GetByIdAsync(goodsReceiptId);
            if (goodsReceipt == null)
                throw new InvalidOperationException($"Goods receipt with ID {goodsReceiptId} not found");

            // Validate purchase order item exists
            var purchaseOrderItem = await _purchaseOrderItemRepository.GetByIdAsync(dto.PurchaseOrderItemId);
            if (purchaseOrderItem == null)
                throw new InvalidOperationException($"Purchase order item with ID {dto.PurchaseOrderItemId} not found");

            // Check if item already exists in this goods receipt
            var existingItems = await _itemRepository.GetByGoodsReceiptIdAsync(goodsReceiptId);
            if (existingItems.Any(i => i.PurchaseOrderItemId == dto.PurchaseOrderItemId))
                throw new InvalidOperationException("This purchase order item already exists in the goods receipt");

            var item = new GoodsReceiptItem
            {
                GoodsReceiptId = goodsReceiptId,
                PurchaseOrderItemId = dto.PurchaseOrderItemId,
                OrderedQuantity = dto.OrderedQuantity,
                ReceivedQuantity = 0,
                AcceptedQuantity = 0,
                RejectedQuantity = 0,
                QualityStatus = QualityStatus.Good,
                Notes = dto.Notes,
                CreatedDate = DateTime.UtcNow
            };

            await _itemRepository.AddAsync(item);
            await _itemRepository.SaveChangesAsync();

            return MapToResponse(item);
        }

        public async Task<GoodsReceiptItemResponse> UpdateAsync(int id, UpdateGoodsReceiptItemDto dto)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item == null)
                throw new InvalidOperationException($"Goods receipt item with ID {id} not found");

            // Only allow updates if goods receipt is not completed
            var goodsReceipt = await _goodsReceiptRepository.GetByIdAsync(item.GoodsReceiptId);
            if (goodsReceipt?.Status == GoodsReceiptStatus.Completed)
                throw new InvalidOperationException("Cannot update items of a completed goods receipt");

            item.OrderedQuantity = dto.OrderedQuantity;
            item.Notes = dto.Notes;
            item.UpdatedDate = DateTime.UtcNow;

            _itemRepository.Update(item);
            await _itemRepository.SaveChangesAsync();

            return MapToResponse(item);
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item == null)
                throw new InvalidOperationException($"Goods receipt item with ID {id} not found");

            // Only allow deletion if goods receipt is pending
            var goodsReceipt = await _goodsReceiptRepository.GetByIdAsync(item.GoodsReceiptId);
            if (goodsReceipt?.Status != GoodsReceiptStatus.Pending)
                throw new InvalidOperationException("Can only delete items from pending goods receipts");

            _itemRepository.Delete(item);
            await _itemRepository.SaveChangesAsync();
        }

        #endregion

        #region Query Operations

        public async Task<IEnumerable<GoodsReceiptItemResponse>> GetByGoodsReceiptIdAsync(int goodsReceiptId)
        {
            var items = await _itemRepository.GetByGoodsReceiptIdAsync(goodsReceiptId);
            return items.Select(MapToResponse);
        }

        public async Task<IEnumerable<GoodsReceiptItemResponse>> GetByPurchaseOrderItemIdAsync(int purchaseOrderItemId)
        {
            var items = await _itemRepository.GetByPurchaseOrderItemIdAsync(purchaseOrderItemId);
            return items.Select(MapToResponse);
        }

        public async Task<GoodsReceiptItemResponse?> GetWithDetailsAsync(int id)
        {
            var item = await _itemRepository.GetWithPurchaseOrderItemAsync(id);
            return item != null ? MapToResponse(item) : null;
        }

        #endregion

        #region Business Operations

        public async Task<GoodsReceiptItemResponse> UpdateQualityInspectionAsync(int id, UpdateQualityInspectionDto dto)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item == null)
                throw new InvalidOperationException($"Goods receipt item with ID {id} not found");

            // Validate quantities
            if (dto.ReceivedQuantity != dto.AcceptedQuantity + dto.RejectedQuantity)
                throw new InvalidOperationException("Received quantity must equal accepted + rejected quantities");

            if (dto.ReceivedQuantity > item.OrderedQuantity)
                throw new InvalidOperationException("Received quantity cannot exceed ordered quantity");

            // Update quality inspection data
            item.ReceivedQuantity = dto.ReceivedQuantity;
            item.AcceptedQuantity = dto.AcceptedQuantity;
            item.RejectedQuantity = dto.RejectedQuantity;
            item.QualityStatus = dto.QualityStatus;
            item.BatchNumber = dto.BatchNumber;
            item.ManufactureDate = dto.ManufactureDate;
            item.ExpiryDate = dto.ExpiryDate;
            item.InspectionNotes = dto.InspectionNotes;
            item.UpdatedDate = DateTime.UtcNow;

            _itemRepository.Update(item);
            await _itemRepository.SaveChangesAsync();

            // Update purchase order item received quantity
            var purchaseOrderItem = await _purchaseOrderItemRepository.GetByIdAsync(item.PurchaseOrderItemId);
            if (purchaseOrderItem != null)
            {
                var allReceiptItems = await _itemRepository.GetByPurchaseOrderItemIdAsync(item.PurchaseOrderItemId);
                purchaseOrderItem.ReceivedQuantity = allReceiptItems.Sum(i => i.AcceptedQuantity);
                _purchaseOrderItemRepository.Update(purchaseOrderItem);
                await _purchaseOrderItemRepository.SaveChangesAsync();
            }

            return MapToResponse(item);
        }

        public async Task<GoodsReceiptItemResponse> UpdateLocationAsync(int id, UpdateItemLocationDto dto)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item == null)
                throw new InvalidOperationException($"Goods receipt item with ID {id} not found");

            item.LocationCode = dto.LocationCode;
            item.ShelfNumber = dto.ShelfNumber;
            if (!string.IsNullOrWhiteSpace(dto.Notes))
                item.Notes = dto.Notes;
            item.UpdatedDate = DateTime.UtcNow;

            _itemRepository.Update(item);
            await _itemRepository.SaveChangesAsync();

            return MapToResponse(item);
        }

        public async Task<GoodsReceiptItemSummaryResponse> GetSummaryByGoodsReceiptIdAsync(int goodsReceiptId)
        {
            var items = await _itemRepository.GetByGoodsReceiptIdAsync(goodsReceiptId);
            var itemsList = items.ToList();

            var totalReceived = itemsList.Sum(i => i.ReceivedQuantity);
            var totalAccepted = itemsList.Sum(i => i.AcceptedQuantity);
            var totalRejected = itemsList.Sum(i => i.RejectedQuantity);

            return new GoodsReceiptItemSummaryResponse
            {
                TotalItems = itemsList.Count,
                TotalOrderedQuantity = itemsList.Sum(i => i.OrderedQuantity),
                TotalReceivedQuantity = totalReceived,
                TotalAcceptedQuantity = totalAccepted,
                TotalRejectedQuantity = totalRejected,
                OverallAcceptanceRate = totalReceived > 0 ? (decimal)totalAccepted / totalReceived * 100 : 0,
                OverallRejectionRate = totalReceived > 0 ? (decimal)totalRejected / totalReceived * 100 : 0,
                ItemsWithGoodQuality = itemsList.Count(i => i.QualityStatus == QualityStatus.Good),
                ItemsWithDamagedQuality = itemsList.Count(i => i.QualityStatus == QualityStatus.Damaged),
                ItemsWithExpiredQuality = itemsList.Count(i => i.QualityStatus == QualityStatus.Expired),
                ItemsWithDefectiveQuality = itemsList.Count(i => i.QualityStatus == QualityStatus.Defective)
            };
        }

        public async Task<IEnumerable<GoodsReceiptItemResponse>> GetItemsByQualityStatusAsync(int goodsReceiptId, QualityStatus qualityStatus)
        {
            var items = await _itemRepository.GetByGoodsReceiptIdAsync(goodsReceiptId);
            return items.Where(i => i.QualityStatus == qualityStatus).Select(MapToResponse);
        }

        public async Task<IEnumerable<GoodsReceiptItemResponse>> GetExpiringSoonItemsAsync(int daysThreshold = 30)
        {
            var items = await _itemRepository.GetAllAsync();
            var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);

            return items
                .Where(i => i.ExpiryDate.HasValue &&
                           i.ExpiryDate.Value <= thresholdDate &&
                           i.ExpiryDate.Value > DateTime.UtcNow &&
                           i.AcceptedQuantity > 0)
                .Select(MapToResponse);
        }

        public async Task<IEnumerable<GoodsReceiptItemResponse>> GetExpiredItemsAsync()
        {
            var items = await _itemRepository.GetAllAsync();
            var now = DateTime.UtcNow;

            return items
                .Where(i => i.ExpiryDate.HasValue &&
                           i.ExpiryDate.Value < now &&
                           i.AcceptedQuantity > 0)
                .Select(MapToResponse);
        }

        #endregion

        #region Mapping

        private GoodsReceiptItemResponse MapToResponse(GoodsReceiptItem item)
        {
            var acceptanceRate = item.ReceivedQuantity > 0
                ? (decimal)item.AcceptedQuantity / item.ReceivedQuantity * 100
                : 0;

            var rejectionRate = item.ReceivedQuantity > 0
                ? (decimal)item.RejectedQuantity / item.ReceivedQuantity * 100
                : 0;

            return new GoodsReceiptItemResponse
            {
                Id = item.Id,
                GoodsReceiptId = item.GoodsReceiptId,
                GoodsReceiptNumber = item.GoodsReceipt?.ReceiptNumber ?? string.Empty,
                PurchaseOrderItemId = item.PurchaseOrderItemId,
                ProductVariantId = item.PurchaseOrderItem?.ProductVariantId ?? 0,
                ProductName = item.PurchaseOrderItem?.ProductName ?? string.Empty,
                SKU = item.PurchaseOrderItem?.SKU ?? string.Empty,
                VariantOptions = item.PurchaseOrderItem?.VariantOptions,
                OrderedQuantity = item.OrderedQuantity,
                ReceivedQuantity = item.ReceivedQuantity,
                AcceptedQuantity = item.AcceptedQuantity,
                RejectedQuantity = item.RejectedQuantity,
                QualityStatus = item.QualityStatus,
                BatchNumber = item.BatchNumber,
                ManufactureDate = item.ManufactureDate,
                ExpiryDate = item.ExpiryDate,
                LocationCode = item.LocationCode,
                ShelfNumber = item.ShelfNumber,
                Notes = item.Notes,
                InspectionNotes = item.InspectionNotes,
                CreatedDate = item.CreatedDate,
                UpdatedDate = item.UpdatedDate,
                AcceptanceRate = acceptanceRate,
                RejectionRate = rejectionRate,
                IsFullyReceived = item.ReceivedQuantity >= item.OrderedQuantity,
                RemainingQuantity = item.OrderedQuantity - item.ReceivedQuantity
            };
        }

        #endregion
    }
}
