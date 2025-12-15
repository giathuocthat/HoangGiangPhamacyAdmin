using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contracts.DTOs.ProductBatch;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class ProductBatchService
    {
        private readonly IRepository<ProductBatch> _productBatchRepository;
        private readonly IProductVariantRepository _productVariantRepository;

        public ProductBatchService(
            IRepository<ProductBatch> productBatchRepository,
            IProductVariantRepository productVariantRepository)
        {
            _productBatchRepository = productBatchRepository;
            _productVariantRepository = productVariantRepository;
        }

        public async Task<BatchLookupResponseDto> GetBatchByNumberAsync(string batchNumber)
        {
            var batch = await _productBatchRepository.FirstOrDefaultAsync(b => b.BatchNumber == batchNumber);
            if (batch == null)
            {
                return null;
            }

            var variant = await _productVariantRepository.GetByIdAsync(batch.ProductVariantId);
            // Ensure product is loaded if not already via repository include (assuming repository might not include it)
            // But helper map uses simple fields. 
            // Better to load with include if possible, but generic repository FirstOrDefaultAsync might not support include easily 
            // unless we cast or use specific method. 
            // Let's assume we might need to fetch variant details if not populated.

            // To be safe and get Product Name:
            var variantWithProduct = await _productVariantRepository.GetVariantWithProduct(batch.ProductVariantId);

            return new BatchLookupResponseDto
            {
                Id = batch.Id,
                BatchNumber = batch.BatchNumber,
                ProductVariantId = batch.ProductVariantId,
                ProductName = variantWithProduct?.Product?.Name ?? "Unknown Product",
                VariantSKU = variantWithProduct?.SKU ?? "Unknown SKU",
                ManufactureDate = batch.ManufactureDate,
                ExpiryDate = batch.ExpiryDate,
                CostPrice = batch.CostPrice,
                QRCodePath = batch.QRCodePath,
                PurchaseOrderNumber = batch.PurchaseOrderNumber,
                Supplier = batch.Supplier,
                IsActive = batch.IsActive
            };
        }

        public async Task<BatchLookupResponseDto> CreateBatchAsync(CreateBatchDto dto, int userId)
        {
            // 1. Check if batch number exists
            var existingBatch = await _productBatchRepository.FirstOrDefaultAsync(b => b.BatchNumber == dto.BatchNumber);
            if (existingBatch != null)
            {
                throw new InvalidOperationException($"Batch number '{dto.BatchNumber}' already exists.");
            }

            // 2. Validate product variant
            var variant = await _productVariantRepository.GetByIdAsync(dto.ProductVariantId);
            if (variant == null)
            {
                throw new InvalidOperationException($"Product variant with ID {dto.ProductVariantId} not found.");
            }

            // 3. Create Batch Entity
            var batch = new ProductBatch
            {
                BatchNumber = dto.BatchNumber,
                ProductVariantId = dto.ProductVariantId,
                ManufactureDate = dto.ManufactureDate,
                ExpiryDate = dto.ExpiryDate,
                CostPrice = dto.CostPrice,
                PurchaseOrderNumber = dto.PurchaseOrderNumber,
                Supplier = dto.Supplier,
                ReceivedDate = DateTime.UtcNow,
                CreatedByUserId = userId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _productBatchRepository.AddAsync(batch);
            await _productBatchRepository.SaveChangesAsync();

            return await GetBatchByNumberAsync(batch.BatchNumber);
        }
    }
}
