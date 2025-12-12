using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class OrderFulfillmentService : IOrderFulfillmentService
    {
        private readonly IOrderFulfillmentRepository _fulfillmentRepository;
        private readonly ILogger<OrderFulfillmentService> _logger;

        public OrderFulfillmentService(
            IOrderFulfillmentRepository fulfillmentRepository,
            ILogger<OrderFulfillmentService> logger)
        {
            _fulfillmentRepository = fulfillmentRepository;
            _logger = logger;
        }

        public async Task<FulfillOrderResponseDto> FulfillOrdersAsync(FulfillOrderRequestDto request, Guid userId)
        {
            var response = new FulfillOrderResponseDto();

            try
            {
                // 1. Lấy danh sách đơn hàng cần fulfill
                var orders = await _fulfillmentRepository.GetPendingOrdersAsync(request.OrderIds);

                if (!orders.Any())
                {
                    _logger.LogInformation("No pending orders found to fulfill");
                    return response;
                }

                _logger.LogInformation($"Found {orders.Count} orders to fulfill");

                // 2. Xử lý từng đơn hàng (đã được sắp xếp theo CreatedDate)
                foreach (var order in orders)
                {
                    try
                    {
                        var orderDetail = await FulfillSingleOrderAsync(order, request.WarehouseId, userId);
                        
                        // Phân loại kết quả
                        if (orderDetail.Items.All(i => i.QuantityPending == 0))
                        {
                            // Fulfill hoàn toàn
                            response.SuccessfulOrders.Add(orderDetail);
                        }
                        else if (orderDetail.Items.Any(i => i.BatchAllocations.Any()))
                        {
                            // Fulfill một phần
                            response.PartialOrders.Add(orderDetail);
                        }
                        else
                        {
                            // Không fulfill được gì
                            response.FailedOrders.Add(new OrderFulfillmentErrorDto
                            {
                                OrderId = order.Id,
                                OrderNumber = order.OrderNumber,
                                ErrorMessage = "Không có hàng trong kho"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error fulfilling order {order.OrderNumber}");
                        response.FailedOrders.Add(new OrderFulfillmentErrorDto
                        {
                            OrderId = order.Id,
                            OrderNumber = order.OrderNumber,
                            ErrorMessage = ex.Message
                        });
                    }
                }

                response.TotalOrdersProcessed = orders.Count;

                // Lưu tất cả thay đổi
                await _fulfillmentRepository.SaveChangesAsync();

                _logger.LogInformation($"Fulfillment completed: {response.SuccessfulOrders.Count} successful, " +
                                     $"{response.PartialOrders.Count} partial, {response.FailedOrders.Count} failed");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FulfillOrdersAsync");
                throw;
            }
        }

        private async Task<OrderFulfillmentDetailDto> FulfillSingleOrderAsync(Order order, int warehouseId, Guid userId)
        {
            var orderDetail = new OrderFulfillmentDetailDto
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber
            };

            foreach (var orderItem in order.OrderItems)
            {
                var itemDto = new OrderItemFulfillmentDto
                {
                    OrderItemId = orderItem.Id,
                    ProductVariantId = orderItem.ProductVariantId,
                    SKU = orderItem.ProductVariant?.SKU ?? "",
                    ProductName = orderItem.ProductVariant?.Product?.Name ?? "",
                    QuantityOrdered = orderItem.Quantity,
                    QuantityFulfilled = orderItem.QuantityFulfilled,
                    QuantityPending = orderItem.QuantityPending
                };

                // Chỉ fulfill nếu còn số lượng pending
                if (orderItem.QuantityPending > 0)
                {
                    var quantityToFulfill = orderItem.QuantityPending;
                    
                    // Lấy danh sách batch available (đã sắp xếp theo FEFO)
                    var availableBatches = await _fulfillmentRepository.GetAvailableBatchesAsync(
                        orderItem.ProductVariantId, 
                        warehouseId);

                    // Book hàng từ các batch theo thứ tự FEFO
                    foreach (var batch in availableBatches)
                    {
                        if (quantityToFulfill <= 0) break;

                        var quantityFromThisBatch = Math.Min(quantityToFulfill, batch.QuantityRemaining);

                        // Tạo OrderItemFulfillment record
                        var fulfillment = new OrderItemFulfillment
                        {
                            OrderItemId = orderItem.Id,
                            InventoryBatchId = batch.Id,
                            QuantityFulfilled = quantityFromThisBatch,
                            FulfilledDate = DateTime.Now,
                            FulfilledByUserId = userId
                        };

                        await _fulfillmentRepository.AddFulfillmentAsync(fulfillment);

                        // Cập nhật InventoryBatch
                        batch.QuantitySold += quantityFromThisBatch;

                        // Cập nhật OrderItem
                        orderItem.QuantityFulfilled += quantityFromThisBatch;

                        // Cập nhật Inventory.QuantityReserved
                        var inventory = await _fulfillmentRepository.GetInventoryAsync(
                            orderItem.ProductVariantId, 
                            warehouseId);
                        
                        if (inventory != null)
                        {
                            inventory.QuantityReserved += quantityFromThisBatch;
                        }

                        // Thêm vào BatchAllocations
                        itemDto.BatchAllocations.Add(new BatchAllocationDto
                        {
                            InventoryBatchId = batch.Id,
                            BatchNumber = batch.BatchNumber,
                            ExpiryDate = batch.ExpiryDate,
                            QuantityAllocated = quantityFromThisBatch
                        });

                        quantityToFulfill -= quantityFromThisBatch;

                        _logger.LogInformation($"Allocated {quantityFromThisBatch} from batch {batch.BatchNumber} " +
                                             $"(Expiry: {batch.ExpiryDate:yyyy-MM-dd}) for OrderItem {orderItem.Id}");
                    }

                    // Cập nhật QuantityFulfilled và QuantityPending trong DTO
                    itemDto.QuantityFulfilled = orderItem.QuantityFulfilled;
                    itemDto.QuantityPending = orderItem.QuantityPending;
                }

                orderDetail.Items.Add(itemDto);
            }

            return orderDetail;
        }
    }
}
