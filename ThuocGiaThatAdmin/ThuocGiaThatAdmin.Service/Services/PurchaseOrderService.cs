using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contract.Responses;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Domain.Enums;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service.Services
{
    /// <summary>
    /// Service implementation for PurchaseOrder business logic
    /// </summary>
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IPurchaseOrderItemRepository _itemRepository;
        private readonly IPurchaseOrderHistoryRepository _historyRepository;
        private readonly IProductVariantRepository _productVariantRepository;

        public PurchaseOrderService(
            IPurchaseOrderRepository purchaseOrderRepository,
            IPurchaseOrderItemRepository itemRepository,
            IPurchaseOrderHistoryRepository historyRepository,
            IProductVariantRepository productVariantRepository)
        {
            _purchaseOrderRepository = purchaseOrderRepository ?? throw new ArgumentNullException(nameof(purchaseOrderRepository));
            _itemRepository = itemRepository ?? throw new ArgumentNullException(nameof(itemRepository));
            _historyRepository = historyRepository ?? throw new ArgumentNullException(nameof(historyRepository));
            _productVariantRepository = productVariantRepository ?? throw new ArgumentNullException(nameof(productVariantRepository));
        }

        public async Task<IEnumerable<PurchaseOrderResponse>> GetAllAsync()
        {
            var orders = await _purchaseOrderRepository.GetAllAsync(x => x.Supplier);
            return orders.Select(MapToDto);
        }

        public async Task<PurchaseOrderResponse?> GetByIdAsync(int id)
        {
            var order = await _purchaseOrderRepository.GetByIdAsync(id);
            return order != null ? MapToDto(order) : null;
        }

        public async Task<PurchaseOrderResponse?> GetByOrderNumberAsync(string orderNumber)
        {
            var order = await _purchaseOrderRepository.GetByOrderNumberAsync(orderNumber);
            return order != null ? MapToDto(order) : null;
        }

        public async Task<PurchaseOrderResponse?> GetWithDetailsAsync(int id)
        {
            var order = await _purchaseOrderRepository.GetWithDetailsAsync(id);
            return order != null ? MapToDto(order) : null;
        }

        public async Task<IEnumerable<PurchaseOrderResponse>> GetBySupplierIdAsync(int supplierId)
        {
            var orders = await _purchaseOrderRepository.GetBySupplierIdAsync(supplierId);
            return orders.Select(MapToDto);
        }

        public async Task<IEnumerable<PurchaseOrderResponse>> GetByWarehouseIdAsync(int warehouseId)
        {
            var orders = await _purchaseOrderRepository.GetByWarehouseIdAsync(warehouseId);
            return orders.Select(MapToDto);
        }

        public async Task<IEnumerable<PurchaseOrderResponse>> GetByStatusAsync(PurchaseOrderStatus status)
        {
            var orders = await _purchaseOrderRepository.GetByStatusAsync(status);
            return orders.Select(MapToDto);
        }

        public async Task<(IEnumerable<PurchaseOrderListItemDto>, int totalCount)> GetPagedPurchaseOrdersAsync(
            int pageNumber = 1,
            int pageSize = 20,
            string? searchTerm = null,
            int? supplierId = null,
            int? warehouseId = null,
            PurchaseOrderStatus? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var (orders, totalCount) = await _purchaseOrderRepository.GetPagedPurchaseOrdersAsync(
                pageNumber, pageSize, searchTerm, supplierId, warehouseId, status, fromDate, toDate);

            var dtos = orders.Select(po => new PurchaseOrderListItemDto
            {
                Id = po.Id,
                OrderNumber = po.OrderNumber,
                SupplierName = po.Supplier?.Name ?? string.Empty,
                WarehouseName = po.Warehouse?.Name ?? string.Empty,
                Status = po.Status,
                PaymentStatus = po.PaymentStatus,
                TotalAmount = po.TotalAmount,
                OrderDate = po.OrderDate,
                ExpectedDeliveryDate = po.ExpectedDeliveryDate,
                TotalItems = po.PurchaseOrderItems?.Count ?? 0,
                ReceivedItems = po.PurchaseOrderItems?.Count(i => i.ReceivedQuantity >= i.OrderedQuantity) ?? 0,
                ReceivedPercentage = CalculateReceivedPercentage(po)
            });

            return (dtos, totalCount);
        }

        public async Task<PurchaseOrderResponse> CreateAsync(CreatePurchaseOrderDto dto, int createdByUserId)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Items == null || !dto.Items.Any())
                throw new ArgumentException("Purchase order must have at least one item");

            // Generate order number
            var orderNumber = await GenerateOrderNumberAsync();

            // Calculate totals
            decimal subTotal = 0;
            decimal taxAmount = 0;

            var order = new PurchaseOrder
            {
                OrderNumber = orderNumber,
                SupplierId = dto.SupplierId,
                SupplierContactId = dto.SupplierContactId,
                WarehouseId = dto.WarehouseId,
                Status = PurchaseOrderStatus.Draft,
                OrderDate = DateTime.UtcNow,
                ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
                ShippingFee = dto.ShippingFee,
                DiscountAmount = dto.DiscountAmount,
                Notes = dto.Notes,
                Terms = dto.Terms,
                CreatedByUserId = createdByUserId
            };

            // Add items
            foreach (var itemDto in dto.Items)
            {
                var variant = await _productVariantRepository.GetByIdAsync(itemDto.ProductVariantId);
                if (variant == null)
                    throw new InvalidOperationException($"Product variant {itemDto.ProductVariantId} not found");

                var itemTotal = itemDto.OrderedQuantity * itemDto.UnitPrice;
                var itemTax = itemTotal * itemDto.TaxRate / 100;

                subTotal += itemTotal;
                taxAmount += itemTax;

                order.PurchaseOrderItems.Add(new PurchaseOrderItem
                {
                    ProductVariantId = itemDto.ProductVariantId,
                    OrderedQuantity = itemDto.OrderedQuantity,
                    ReceivedQuantity = 0,
                    UnitPrice = itemDto.UnitPrice,
                    TaxRate = itemDto.TaxRate,
                    DiscountAmount = itemDto.DiscountAmount,
                    TotalAmount = itemTotal + itemTax - itemDto.DiscountAmount,
                    ProductName = variant.Product?.Name ?? string.Empty,
                    SKU = variant.SKU,
                    Notes = itemDto.Notes
                });
            }

            order.SubTotal = subTotal;
            order.TaxAmount = taxAmount;
            order.TotalAmount = subTotal + taxAmount + dto.ShippingFee - dto.DiscountAmount;

            await _purchaseOrderRepository.AddAsync(order);
            await _purchaseOrderRepository.SaveChangesAsync();

            // Add history
            await AddHistoryAsync(order.Id, null, PurchaseOrderStatus.Draft.ToString(),
                "Created", createdByUserId, "Purchase order created");

            return MapToDto(order);
        }

        public async Task<PurchaseOrderResponse> UpdateAsync(int id, UpdatePurchaseOrderDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var order = await _purchaseOrderRepository.GetWithDetailsAsync(id);
            if (order == null)
                throw new InvalidOperationException($"Purchase order {id} not found");

            // FIX: Change OR to AND
            if (order.Status != PurchaseOrderStatus.Draft && order.Status != PurchaseOrderStatus.Pending)
                throw new InvalidOperationException("Only Draft/Pending orders can be updated");

            // Update basic info
            order.SupplierId = dto.SupplierId ?? order.SupplierId;
            order.SupplierContactId = dto.SupplierContactId;
            order.ExpectedDeliveryDate = dto.ExpectedDeliveryDate ?? order.ExpectedDeliveryDate;
            order.ShippingFee = dto.ShippingFee;
            order.DiscountAmount = dto.DiscountAmount;
            order.Notes = dto.Notes;
            order.Terms = dto.Terms;

            // Update payment status if provided
            if (dto.PaymentStatus.HasValue)
            {
                order.PaymentStatus = dto.PaymentStatus.Value;
            }

            // FIX: Update items properly
            if (dto.Items != null && dto.Items.Any())
            {
                decimal subTotal = 0;
                decimal taxAmount = 0;

                // Process each item from DTO
                foreach (var itemDto in dto.Items)
                {
                    // Validation
                    if (itemDto.OrderedQuantity <= 0)
                        throw new InvalidOperationException("OrderedQuantity must be greater than zero.");
                    if (itemDto.UnitPrice < 0)
                        throw new InvalidOperationException("UnitPrice must be greater than or equal to zero.");

                    // Find existing item by its database ID
                    var existingItem = order.PurchaseOrderItems
                        .FirstOrDefault(x => x.Id == itemDto.Id);

                    var itemTotal = itemDto.OrderedQuantity * itemDto.UnitPrice;
                    var itemTax = itemTotal * itemDto.TaxRate / 100;
                    subTotal += itemTotal;
                    taxAmount += itemTax;

                    if (existingItem != null)
                    {
                        // Update existing item
                        existingItem.OrderedQuantity = itemDto.OrderedQuantity;
                        existingItem.UnitPrice = itemDto.UnitPrice;
                        existingItem.TaxRate = itemDto.TaxRate;
                        existingItem.DiscountAmount = itemDto.DiscountAmount;
                        existingItem.TotalAmount = itemTotal + itemTax - itemDto.DiscountAmount;
                        existingItem.Notes = itemDto.Notes;

                        _itemRepository.Update(existingItem);
                    }
                    else
                    {
                        // Add new item (if itemDto.Id == 0 or not found)
                        var variant = await _productVariantRepository.GetByIdAsync(itemDto.ProductVariantId);
                        if (variant == null)
                            throw new InvalidOperationException($"Product variant {itemDto.ProductVariantId} not found");

                        var newItem = new PurchaseOrderItem
                        {
                            PurchaseOrderId = order.Id,
                            ProductVariantId = itemDto.ProductVariantId,
                            OrderedQuantity = itemDto.OrderedQuantity,
                            ReceivedQuantity = 0,
                            UnitPrice = itemDto.UnitPrice,
                            TaxRate = itemDto.TaxRate,
                            DiscountAmount = itemDto.DiscountAmount,
                            TotalAmount = itemTotal + itemTax - itemDto.DiscountAmount,
                            ProductName = variant.Product?.Name ?? string.Empty,
                            SKU = variant.SKU,
                            Notes = itemDto.Notes
                        };

                        order.PurchaseOrderItems.Add(newItem);
                    }
                }

                // Remove items that are not in the DTO (user deleted them)
                var itemIdsFromDto = dto.Items.Where(x => x.Id > 0).Select(x => x.Id).ToList();
                var itemsToRemove = order.PurchaseOrderItems
                    .Where(x => !itemIdsFromDto.Contains(x.Id))
                    .ToList();

                foreach (var itemToRemove in itemsToRemove)
                {
                    order.PurchaseOrderItems.Remove(itemToRemove);
                    _itemRepository.Delete(itemToRemove);
                }

                order.SubTotal = subTotal;
                order.TaxAmount = taxAmount;
                order.TotalAmount = subTotal + taxAmount + dto.ShippingFee - dto.DiscountAmount;
            }

            _purchaseOrderRepository.Update(order);
            await _purchaseOrderRepository.SaveChangesAsync();

            return MapToDto(order);
        }

        public async Task<PurchaseOrderResponse> ApproveAsync(int id, int approvedByUserId, ApprovePurchaseOrderDto dto)
        {
            var order = await _purchaseOrderRepository.GetByIdAsync(id);
            if (order == null)
                throw new InvalidOperationException($"Purchase order {id} not found");

            if (order.Status != PurchaseOrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be approved");

            var oldStatus = order.Status.ToString();
            order.Status = PurchaseOrderStatus.Approved;
            order.ApprovedByUserId = approvedByUserId;
            order.ApprovedDate = DateTime.UtcNow;

            _purchaseOrderRepository.Update(order);
            await _purchaseOrderRepository.SaveChangesAsync();

            await AddHistoryAsync(id, oldStatus, order.Status.ToString(),
                "Approved", approvedByUserId, dto.Notes);

            return MapToDto(order);
        }

        public async Task<PurchaseOrderResponse> CancelAsync(int id, int cancelledByUserId, CancelPurchaseOrderDto dto)
        {
            var order = await _purchaseOrderRepository.GetByIdAsync(id);
            if (order == null)
                throw new InvalidOperationException($"Purchase order {id} not found");

            if (order.Status == PurchaseOrderStatus.Completed || order.Status == PurchaseOrderStatus.Cancelled)
                throw new InvalidOperationException("Cannot cancel completed or already cancelled orders");

            var oldStatus = order.Status.ToString();
            order.Status = PurchaseOrderStatus.Cancelled;

            _purchaseOrderRepository.Update(order);
            await _purchaseOrderRepository.SaveChangesAsync();

            await AddHistoryAsync(id, oldStatus, order.Status.ToString(),
                "Cancelled", cancelledByUserId, dto.Reason);

            return MapToDto(order);
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _purchaseOrderRepository.GetByIdAsync(id);
            if (order == null)
                throw new InvalidOperationException($"Purchase order {id} not found");

            if (order.Status != PurchaseOrderStatus.Draft)
                throw new InvalidOperationException("Only draft orders can be deleted");

            _purchaseOrderRepository.Delete(order);
            await _purchaseOrderRepository.SaveChangesAsync();
        }

        public async Task<string> GenerateOrderNumberAsync()
        {
            return await _purchaseOrderRepository.GenerateOrderNumberAsync();
        }

        private async Task AddHistoryAsync(int purchaseOrderId, string? fromStatus, string toStatus,
            string action, int changedByUserId, string? notes)
        {
            var history = new PurchaseOrderHistory
            {
                PurchaseOrderId = purchaseOrderId,
                FromStatus = fromStatus,
                ToStatus = toStatus,
                Action = action,
                ChangedByUserId = changedByUserId,
                ChangedDate = DateTime.UtcNow,
                Notes = notes
            };

            await _historyRepository.AddAsync(history);
            await _historyRepository.SaveChangesAsync();
        }

        private decimal CalculateReceivedPercentage(PurchaseOrder order)
        {
            if (order.PurchaseOrderItems == null || !order.PurchaseOrderItems.Any())
                return 0;

            var totalOrdered = order.PurchaseOrderItems.Sum(i => i.OrderedQuantity);
            var totalReceived = order.PurchaseOrderItems.Sum(i => i.ReceivedQuantity);

            return totalOrdered > 0 ? (decimal)totalReceived / totalOrdered * 100 : 0;
        }

        private PurchaseOrderResponse MapToDto(PurchaseOrder order)
        {
            return new PurchaseOrderResponse
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                SupplierId = order.SupplierId,
                SupplierName = order.Supplier?.Name ?? string.Empty,
                SupplierContactId = order.SupplierContactId,
                SupplierContactName = order.SupplierContact?.FullName,
                WarehouseId = order.WarehouseId,
                WarehouseName = order.Warehouse?.Name ?? string.Empty,
                Status = order.Status,
                StatusName = order.Status.ToString(),
                PaymentStatus = order.PaymentStatus,
                PaymentStatusName = order.PaymentStatus.ToString(),
                SubTotal = order.SubTotal,
                TaxAmount = order.TaxAmount,
                DiscountAmount = order.DiscountAmount,
                ShippingFee = order.ShippingFee,
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                ExpectedDeliveryDate = order.ExpectedDeliveryDate,
                CompletedDate = order.CompletedDate,
                CreatedByUserId = order.CreatedByUserId,
                ApprovedByUserId = order.ApprovedByUserId,
                ApprovedDate = order.ApprovedDate,
                Notes = order.Notes,
                Terms = order.Terms,
                CreatedDate = order.CreatedDate,
                UpdatedDate = order.UpdatedDate,
                Items = order.PurchaseOrderItems?.Select(i => new PurchaseOrderItemResponse
                {
                    Id = i.Id,
                    PurchaseOrderId = i.PurchaseOrderId,
                    ProductVariantId = i.ProductVariantId,
                    OrderedQuantity = i.OrderedQuantity,
                    ReceivedQuantity = i.ReceivedQuantity,
                    UnitPrice = i.UnitPrice,
                    TaxRate = i.TaxRate,
                    DiscountAmount = i.DiscountAmount,
                    TotalAmount = i.TotalAmount,
                    ProductName = i.ProductName,
                    SKU = i.SKU,
                    VariantOptions = i.VariantOptions,
                    Notes = i.Notes,
                    RemainingQuantity = i.OrderedQuantity - i.ReceivedQuantity,
                    ReceivedPercentage = i.OrderedQuantity > 0 ? (decimal)i.ReceivedQuantity / i.OrderedQuantity * 100 : 0
                }).ToList() ?? new List<PurchaseOrderItemResponse>(),
                GoodsReceipts = order.GoodsReceipts?.Select(gr => new GoodReceiptResponse
                {
                    Id = gr.Id,
                    ReceiptNumber = gr.ReceiptNumber,
                    Status = gr.Status,
                    ScheduledDate = gr.ScheduledDate,
                    ReceivedDate = gr.ReceivedDate
                }).ToList() ?? new List<GoodReceiptResponse>()
            };
        }
    }
}
