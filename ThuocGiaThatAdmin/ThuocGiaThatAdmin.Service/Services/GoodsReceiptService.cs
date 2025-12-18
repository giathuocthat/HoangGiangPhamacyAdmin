using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Domain.Enums;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service.Services
{
    /// <summary>
    /// Service implementation for GoodsReceipt business logic
    /// </summary>
    public class GoodsReceiptService : IGoodsReceiptService
    {
        private readonly IGoodsReceiptRepository _goodsReceiptRepository;
        private readonly IGoodsReceiptItemRepository _itemRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IPurchaseOrderItemRepository _purchaseOrderItemRepository;

        public GoodsReceiptService(
            IGoodsReceiptRepository goodsReceiptRepository,
            IGoodsReceiptItemRepository itemRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
            IPurchaseOrderItemRepository purchaseOrderItemRepository)
        {
            _goodsReceiptRepository = goodsReceiptRepository ?? throw new ArgumentNullException(nameof(goodsReceiptRepository));
            _itemRepository = itemRepository ?? throw new ArgumentNullException(nameof(itemRepository));
            _purchaseOrderRepository = purchaseOrderRepository ?? throw new ArgumentNullException(nameof(purchaseOrderRepository));
            _purchaseOrderItemRepository = purchaseOrderItemRepository ?? throw new ArgumentNullException(nameof(purchaseOrderItemRepository));
        }

        public async Task<IEnumerable<GoodsReceiptDto>> GetAllAsync()
        {
            var receipts = await _goodsReceiptRepository.GetAllAsync( x => x.PurchaseOrder, x=> x.Warehouse);
            return receipts.Select(MapToDto);
        }

        public async Task<GoodsReceiptDto?> GetByIdAsync(int id)
        {
            var receipt = await _goodsReceiptRepository.GetByIdAsync(id);
            return receipt != null ? MapToDto(receipt) : null;
        }

        public async Task<GoodsReceiptDto?> GetByReceiptNumberAsync(string receiptNumber)
        {
            var receipt = await _goodsReceiptRepository.GetByReceiptNumberAsync(receiptNumber);
            return receipt != null ? MapToDto(receipt) : null;
        }

        public async Task<GoodsReceiptDto?> GetWithDetailsAsync(int id)
        {
            var receipt = await _goodsReceiptRepository.GetWithDetailsAsync(id);
            return receipt != null ? MapToDto(receipt) : null;
        }

        public async Task<IEnumerable<GoodsReceiptDto>> GetByPurchaseOrderIdAsync(int purchaseOrderId)
        {
            var receipts = await _goodsReceiptRepository.GetByPurchaseOrderIdAsync(purchaseOrderId);
            return receipts.Select(MapToDto);
        }

        public async Task<IEnumerable<GoodsReceiptDto>> GetByWarehouseIdAsync(int warehouseId)
        {
            var receipts = await _goodsReceiptRepository.GetByWarehouseIdAsync(warehouseId);
            return receipts.Select(MapToDto);
        }

        public async Task<IEnumerable<GoodsReceiptDto>> GetByStatusAsync(GoodsReceiptStatus status)
        {
            var receipts = await _goodsReceiptRepository.GetByStatusAsync(status);
            return receipts.Select(MapToDto);
        }

        public async Task<(IEnumerable<GoodsReceiptListItemDto> receipts, int totalCount)> GetFilteredGoodsReceiptsAsync(FilterRequest request)
        {
            var (receipts, totalCount) = await _goodsReceiptRepository.GetFilteredGoodsReceiptsAsync(request);

            var dtos = receipts.Select(gr => new GoodsReceiptListItemDto
            {
                Id = gr.Id,
                ReceiptNumber = gr.ReceiptNumber,
                PurchaseOrderNumber = gr.PurchaseOrder?.OrderNumber ?? string.Empty,
                WarehouseName = gr.Warehouse?.Name ?? string.Empty,
                Status = gr.Status,
                ScheduledDate = gr.ScheduledDate,
                ReceivedDate = gr.ReceivedDate,
                TotalItems = gr.GoodsReceiptItems?.Count ?? 0,
                AcceptedItems = gr.GoodsReceiptItems?.Count(i => i.QualityStatus == QualityStatus.Good) ?? 0,
                RejectedItems = gr.GoodsReceiptItems?.Count(i => i.RejectedQuantity > 0) ?? 0
            });

            return (dtos, totalCount);
        }

        public async Task<(IEnumerable<GoodsReceiptListItemDto>, int totalCount)> GetPagedGoodsReceiptsAsync(
            int pageNumber = 1,
            int pageSize = 20,
            string? searchTerm = null,
            int? purchaseOrderId = null,
            int? warehouseId = null,
            GoodsReceiptStatus? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var (receipts, totalCount) = await _goodsReceiptRepository.GetPagedGoodsReceiptsAsync(
                pageNumber, pageSize, searchTerm, purchaseOrderId, warehouseId, status, fromDate, toDate);

            var dtos = receipts.Select(gr => new GoodsReceiptListItemDto
            {
                Id = gr.Id,
                ReceiptNumber = gr.ReceiptNumber,
                PurchaseOrderNumber = gr.PurchaseOrder?.OrderNumber ?? string.Empty,
                WarehouseName = gr.Warehouse?.Name ?? string.Empty,
                Status = gr.Status,
                ScheduledDate = gr.ScheduledDate,
                ReceivedDate = gr.ReceivedDate,
                TotalItems = gr.GoodsReceiptItems?.Count ?? 0,
                AcceptedItems = gr.GoodsReceiptItems?.Count(i => i.QualityStatus == QualityStatus.Good) ?? 0,
                RejectedItems = gr.GoodsReceiptItems?.Count(i => i.RejectedQuantity > 0) ?? 0
            });

            return (dtos, totalCount);
        }

        public async Task<GoodsReceiptDto> CreateAsync(CreateGoodsReceiptDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Items == null || !dto.Items.Any())
                throw new ArgumentException("Goods receipt must have at least one item");

            // Verify purchase order exists
            var purchaseOrder = await _purchaseOrderRepository.GetWithDetailsAsync(dto.PurchaseOrderId);
            if (purchaseOrder == null)
                throw new InvalidOperationException($"Purchase order {dto.PurchaseOrderId} not found");

            if (purchaseOrder.Status != PurchaseOrderStatus.Approved &&
                purchaseOrder.Status != PurchaseOrderStatus.PartiallyReceived)
                throw new InvalidOperationException("Purchase order must be approved before creating goods receipt");

            // Generate receipt number
            var receiptNumber = await GenerateReceiptNumberAsync();

            var receipt = new GoodsReceipt
            {
                ReceiptNumber = receiptNumber,
                PurchaseOrderId = dto.PurchaseOrderId,
                WarehouseId = dto.WarehouseId,
                Status = GoodsReceiptStatus.Pending,
                ScheduledDate = dto.ScheduledDate,
                ShippingCarrier = dto.ShippingCarrier,
                TrackingNumber = dto.TrackingNumber,
                VehicleNumber = dto.VehicleNumber,
                Notes = dto.Notes
            };

            // Add items
            foreach (var itemDto in dto.Items)
            {
                var poItem = purchaseOrder.PurchaseOrderItems?.FirstOrDefault(i => i.Id == itemDto.PurchaseOrderItemId);
                if (poItem == null)
                    throw new InvalidOperationException($"Purchase order item {itemDto.PurchaseOrderItemId} not found");

                receipt.GoodsReceiptItems.Add(new GoodsReceiptItem
                {
                    PurchaseOrderItemId = itemDto.PurchaseOrderItemId,
                    OrderedQuantity = itemDto.OrderedQuantity,
                    ReceivedQuantity = 0,
                    AcceptedQuantity = 0,
                    QualityStatus = QualityStatus.Good,
                    RejectedQuantity = 0,
                    Notes = itemDto.Notes
                });
            }

            await _goodsReceiptRepository.AddAsync(receipt);
            await _goodsReceiptRepository.SaveChangesAsync();

            return MapToDto(receipt);
        }

        public async Task<GoodsReceiptDto> UpdateAsync(int id, UpdateGoodsReceiptDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var receipt = await _goodsReceiptRepository.GetByIdAsync(id);
            if (receipt == null)
                throw new InvalidOperationException($"Goods receipt {id} not found");

            if (receipt.Status != GoodsReceiptStatus.Pending)
                throw new InvalidOperationException("Only pending receipts can be updated");

            receipt.ScheduledDate = dto.ScheduledDate;
            receipt.ShippingCarrier = dto.ShippingCarrier;
            receipt.TrackingNumber = dto.TrackingNumber;
            receipt.VehicleNumber = dto.VehicleNumber;
            receipt.Notes = dto.Notes;

            _goodsReceiptRepository.Update(receipt);
            await _goodsReceiptRepository.SaveChangesAsync();

            return MapToDto(receipt);
        }

        public async Task<GoodsReceiptDto> ReceiveGoodsAsync(int id, ReceiveGoodsDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var receipt = await _goodsReceiptRepository.GetWithDetailsAsync(id);
            if (receipt == null)
                throw new InvalidOperationException($"Goods receipt {id} not found");

            if (receipt.Status != GoodsReceiptStatus.Pending && receipt.Status != GoodsReceiptStatus.InTransit)
                throw new InvalidOperationException("Only pending or in-transit receipts can receive goods");

            receipt.Status = GoodsReceiptStatus.Received;
            receipt.ReceivedDate = dto.ReceivedDate;
            receipt.ReceivedByUserId = dto.ReceivedByUserId;
            receipt.InspectedByUserId = dto.InspectedByUserId;
            receipt.Notes = dto.Notes;

            // Update items with received quantities
            foreach (var itemDto in dto.Items)
            {
                var item = receipt.GoodsReceiptItems?.FirstOrDefault(i => i.Id == itemDto.GoodsReceiptItemId);
                if (item == null)
                    throw new InvalidOperationException($"Goods receipt item {itemDto.GoodsReceiptItemId} not found");

                item.ReceivedQuantity = itemDto.ReceivedQuantity;
                item.AcceptedQuantity = itemDto.AcceptedQuantity;
                item.RejectedQuantity = itemDto.RejectedQuantity;
                item.QualityStatus = itemDto.QualityStatus;
                item.BatchNumber = itemDto.BatchNumber;
                item.ManufactureDate = itemDto.ManufactureDate;
                item.ExpiryDate = itemDto.ExpiryDate;
                item.LocationCode = itemDto.LocationCode;
                item.ShelfNumber = itemDto.ShelfNumber;
                item.InspectionNotes = itemDto.InspectionNotes;

                // Update PurchaseOrderItem received quantity
                var poItem = await _purchaseOrderItemRepository.GetByIdAsync(item.PurchaseOrderItemId);
                if (poItem != null)
                {
                    poItem.ReceivedQuantity += itemDto.AcceptedQuantity;
                    _purchaseOrderItemRepository.Update(poItem);
                }
            }

            _goodsReceiptRepository.Update(receipt);
            await _goodsReceiptRepository.SaveChangesAsync();

            // Update purchase order status
            await UpdatePurchaseOrderStatusAsync(receipt.PurchaseOrderId);

            return MapToDto(receipt);
        }

        public async Task<GoodsReceiptDto> CompleteAsync(int id, CompleteGoodsReceiptDto dto)
        {
            var receipt = await _goodsReceiptRepository.GetByIdAsync(id);
            if (receipt == null)
                throw new InvalidOperationException($"Goods receipt {id} not found");

            if (receipt.Status != GoodsReceiptStatus.Received)
                throw new InvalidOperationException("Only received receipts can be completed");

            receipt.Status = GoodsReceiptStatus.Completed;
            receipt.CompletedDate = DateTime.UtcNow;
            receipt.Notes = dto.Notes;

            _goodsReceiptRepository.Update(receipt);
            await _goodsReceiptRepository.SaveChangesAsync();

            return MapToDto(receipt);
        }

        public async Task<GoodsReceiptDto> RejectAsync(int id, RejectGoodsReceiptDto dto)
        {
            var receipt = await _goodsReceiptRepository.GetByIdAsync(id);
            if (receipt == null)
                throw new InvalidOperationException($"Goods receipt {id} not found");

            receipt.Status = GoodsReceiptStatus.Rejected;
            receipt.RejectionReason = dto.RejectionReason;

            _goodsReceiptRepository.Update(receipt);
            await _goodsReceiptRepository.SaveChangesAsync();

            return MapToDto(receipt);
        }

        public async Task DeleteAsync(int id)
        {
            var receipt = await _goodsReceiptRepository.GetByIdAsync(id);
            if (receipt == null)
                throw new InvalidOperationException($"Goods receipt {id} not found");

            if (receipt.Status != GoodsReceiptStatus.Pending)
                throw new InvalidOperationException("Only pending receipts can be deleted");

            _goodsReceiptRepository.Delete(receipt);
            await _goodsReceiptRepository.SaveChangesAsync();
        }

        public async Task<string> GenerateReceiptNumberAsync()
        {
            return await _goodsReceiptRepository.GenerateReceiptNumberAsync();
        }

        private async Task UpdatePurchaseOrderStatusAsync(int purchaseOrderId)
        {
            var purchaseOrder = await _purchaseOrderRepository.GetWithDetailsAsync(purchaseOrderId);
            if (purchaseOrder == null) return;

            var allItemsReceived = purchaseOrder.PurchaseOrderItems?.All(i => i.ReceivedQuantity >= i.OrderedQuantity) ?? false;
            var anyItemsReceived = purchaseOrder.PurchaseOrderItems?.Any(i => i.ReceivedQuantity > 0) ?? false;

            if (allItemsReceived)
            {
                purchaseOrder.Status = PurchaseOrderStatus.Completed;
                purchaseOrder.CompletedDate = DateTime.UtcNow;
            }
            else if (anyItemsReceived)
            {
                purchaseOrder.Status = PurchaseOrderStatus.PartiallyReceived;
            }

            _purchaseOrderRepository.Update(purchaseOrder);
            await _purchaseOrderRepository.SaveChangesAsync();
        }

        private GoodsReceiptDto MapToDto(GoodsReceipt receipt)
        {
            return new GoodsReceiptDto
            {
                Id = receipt.Id,
                ReceiptNumber = receipt.ReceiptNumber,
                PurchaseOrderId = receipt.PurchaseOrderId,
                PurchaseOrderNumber = receipt.PurchaseOrder?.OrderNumber ?? string.Empty,
                WarehouseId = receipt.WarehouseId,
                WarehouseName = receipt.Warehouse?.Name ?? string.Empty,
                Status = receipt.Status,
                ScheduledDate = receipt.ScheduledDate,
                ReceivedDate = receipt.ReceivedDate,
                CompletedDate = receipt.CompletedDate,
                ReceivedByUserId = receipt.ReceivedByUserId,
                InspectedByUserId = receipt.InspectedByUserId,
                ShippingCarrier = receipt.ShippingCarrier,
                TrackingNumber = receipt.TrackingNumber,
                VehicleNumber = receipt.VehicleNumber,
                Notes = receipt.Notes,
                RejectionReason = receipt.RejectionReason,
                CreatedDate = receipt.CreatedDate,
                UpdatedDate = receipt.UpdatedDate,
                Items = receipt.GoodsReceiptItems?.Select(i => new GoodsReceiptItemDto
                {
                    Id = i.Id,
                    GoodsReceiptId = i.GoodsReceiptId,
                    PurchaseOrderItemId = i.PurchaseOrderItemId,
                    ProductVariantId = i.PurchaseOrderItem?.ProductVariantId ?? 0,
                    ProductName = i.PurchaseOrderItem?.ProductName ?? string.Empty,
                    SKU = i.PurchaseOrderItem?.SKU ?? string.Empty,
                    OrderedQuantity = i.OrderedQuantity,
                    ReceivedQuantity = i.ReceivedQuantity,
                    AcceptedQuantity = i.AcceptedQuantity,
                    QualityStatus = i.QualityStatus,
                    RejectedQuantity = i.RejectedQuantity,
                    BatchNumber = i.BatchNumber,
                    ManufactureDate = i.ManufactureDate,
                    ExpiryDate = i.ExpiryDate,
                    LocationCode = i.LocationCode,
                    ShelfNumber = i.ShelfNumber,
                    Notes = i.Notes,
                    InspectionNotes = i.InspectionNotes
                }).ToList() ?? new List<GoodsReceiptItemDto>()
            };
        }
    }
}
