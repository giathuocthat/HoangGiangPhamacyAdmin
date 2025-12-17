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
    /// <summary>
    /// Service implementation cho warehouse picking operations
    /// </summary>
    public class WarehousePickingService : IWarehousePickingService
    {
        private readonly IWarehousePickingRepository _pickingRepository;
        private readonly ILogger<WarehousePickingService> _logger;

        public WarehousePickingService(
            IWarehousePickingRepository pickingRepository,
            ILogger<WarehousePickingService> logger)
        {
            _pickingRepository = pickingRepository;
            _logger = logger;
        }

        public async Task<ProcessPickingResponseDto> ProcessPickingAsync(
            ProcessPickingRequestDto request, 
            Guid userId)
        {
            var response = new ProcessPickingResponseDto();

            try
            {
                _logger.LogInformation($"Processing {request.Movements.Count} picking movements for warehouse {request.WarehouseId}");

                // Xử lý từng movement
                foreach (var movementRequest in request.Movements)
                {
                    try
                    {
                        var result = await ProcessSingleMovementAsync(
                            movementRequest, 
                            request.WarehouseId,
                            request.DestinationLocationCode,
                            userId,
                            request.Notes);

                        response.Movements.Add(result);

                        if (result.Status == "Success")
                        {
                            response.SuccessfulMovements++;
                        }
                        else
                        {
                            response.FailedMovements++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error processing movement from {movementRequest.FromLocationCode}, batch {movementRequest.BatchNumber}");
                        
                        response.Movements.Add(new PickingMovementResultDto
                        {
                            FromLocationCode = movementRequest.FromLocationCode,
                            ToLocationCode = request.DestinationLocationCode,
                            BatchNumber = movementRequest.BatchNumber,
                            Quantity = movementRequest.Quantity,
                            Status = "Failed",
                            ErrorMessage = ex.Message
                        });
                        
                        response.FailedMovements++;
                    }
                }

                response.TotalMovements = request.Movements.Count;

                // Lưu tất cả thay đổi
                await _pickingRepository.SaveChangesAsync();

                _logger.LogInformation($"Picking completed: {response.SuccessfulMovements} successful, {response.FailedMovements} failed");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProcessPickingAsync");
                throw;
            }
        }

        private async Task<PickingMovementResultDto> ProcessSingleMovementAsync(
            PickingMovementRequestDto movementRequest,
            int warehouseId,
            string destinationLocationCode,
            Guid userId,
            string? notes)
        {
            // 1. Lấy BatchLocationStock tại vị trí nguồn
            var sourceStock = await _pickingRepository.GetBatchLocationAsync(
                movementRequest.FromLocationCode,
                movementRequest.BatchNumber,
                warehouseId);

            if (sourceStock == null)
            {
                return new PickingMovementResultDto
                {
                    FromLocationCode = movementRequest.FromLocationCode,
                    ToLocationCode = destinationLocationCode,
                    BatchNumber = movementRequest.BatchNumber,
                    Quantity = movementRequest.Quantity,
                    Status = "Failed",
                    ErrorMessage = $"Batch {movementRequest.BatchNumber} not found at location {movementRequest.FromLocationCode}"
                };
            }

            // 2. Validate số lượng available
            if (sourceStock.QuantityAvailable < movementRequest.Quantity)
            {
                return new PickingMovementResultDto
                {
                    FromLocationCode = movementRequest.FromLocationCode,
                    ToLocationCode = destinationLocationCode,
                    BatchNumber = movementRequest.BatchNumber,
                    ProductVariantId = sourceStock.ProductVariantId,
                    SKU = sourceStock.ProductVariant?.SKU ?? "",
                    ProductName = sourceStock.ProductVariant?.Product?.Name ?? "",
                    Quantity = movementRequest.Quantity,
                    Status = "Failed",
                    ErrorMessage = $"Insufficient stock at location {movementRequest.FromLocationCode}. Available: {sourceStock.QuantityAvailable}, Requested: {movementRequest.Quantity}"
                };
            }

            // 3. Tạo LocationStockMovement record
            var movement = new LocationStockMovement
            {
                ProductVariantId = sourceStock.ProductVariantId,
                WarehouseId = warehouseId,
                FromLocationCode = movementRequest.FromLocationCode,
                ToLocationCode = destinationLocationCode,
                BatchNumber = movementRequest.BatchNumber,
                Quantity = movementRequest.Quantity,
                Reason = notes ?? "Order Picking",
                MovedByUserId = (int?)null, // TODO: Convert Guid to int if needed
                MovementDate = DateTime.Now,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            await _pickingRepository.AddMovementAsync(movement);

            // 4. Cập nhật source BatchLocationStock (giảm quantity)
            sourceStock.Quantity -= movementRequest.Quantity;
            sourceStock.UpdatedDate = DateTime.Now;
            await _pickingRepository.UpdateBatchLocationStockAsync(sourceStock);

            // 5. Cập nhật destination BatchLocationStock (tăng quantity)
            var destinationStock = await _pickingRepository.GetOrCreateDestinationStockAsync(
                sourceStock.InventoryBatchId,
                sourceStock.ProductVariantId,
                warehouseId,
                destinationLocationCode);

            destinationStock.Quantity += movementRequest.Quantity;
            destinationStock.UpdatedDate = DateTime.Now;
            await _pickingRepository.UpdateBatchLocationStockAsync(destinationStock);

            _logger.LogInformation($"Moved {movementRequest.Quantity} of batch {movementRequest.BatchNumber} from {movementRequest.FromLocationCode} to {destinationLocationCode}");

            // 6. Return success result
            return new PickingMovementResultDto
            {
                MovementId = movement.Id,
                FromLocationCode = movementRequest.FromLocationCode,
                ToLocationCode = destinationLocationCode,
                BatchNumber = movementRequest.BatchNumber,
                ProductVariantId = sourceStock.ProductVariantId,
                SKU = sourceStock.ProductVariant?.SKU ?? "",
                ProductName = sourceStock.ProductVariant?.Product?.Name ?? "",
                Quantity = movementRequest.Quantity,
                Status = "Success"
            };
        }

        public async Task<ValidatePickingResponseDto> ValidatePickingAsync(
            string locationCode, 
            string batchNumber, 
            int warehouseId)
        {
            try
            {
                var batchLocation = await _pickingRepository.GetBatchLocationAsync(
                    locationCode,
                    batchNumber,
                    warehouseId);

                if (batchLocation == null)
                {
                    return new ValidatePickingResponseDto
                    {
                        IsValid = false,
                        ErrorMessage = $"Batch {batchNumber} not found at location {locationCode}"
                    };
                }

                return new ValidatePickingResponseDto
                {
                    IsValid = true,
                    LocationCode = locationCode,
                    BatchNumber = batchNumber,
                    ProductVariantId = batchLocation.ProductVariantId,
                    SKU = batchLocation.ProductVariant?.SKU,
                    ProductName = batchLocation.ProductVariant?.Product?.Name,
                    AvailableQuantity = batchLocation.QuantityAvailable,
                    ExpiryDate = batchLocation.InventoryBatch?.ExpiryDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating location {locationCode}, batch {batchNumber}");
                
                return new ValidatePickingResponseDto
                {
                    IsValid = false,
                    ErrorMessage = $"Error validating: {ex.Message}"
                };
            }
        }
    }
}
