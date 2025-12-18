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

        public async Task<ThuocGiaThat.Infrastucture.Common.PagedResult<ProductBatchResponseDto>> GetProductBatchesAsync(GetProductBatchesRequestDto request)
        {
            // We need to include ProductVariant and its Product to get the names
            // However, the generic repository GetPagedAsync includes uses simple includes.
            // Complex includes (ThenInclude) might not be directly supported by the simple params array if not using string paths.
            // But let's try using string paths if the generic repository supported it, but our new GetPagedAsync takes Expression<Func<T, object>>[].
            // This is good for direct properties. For nested, we might need a different approach or update repository to accept string includes, 
            // OR just map it afterwards if the data set is small page size.
            
            // To support ThenInclude correctly via params Expression, we'd need a specific specification pattern or custom repository method.
            // Given the limited scope, I will fetch paged batches first, then enrich the data.
            // OR, since ProductBatch -> ProductVariant, we can include ProductVariant.
            // But getting ProductVariant -> Product (Name) requires ThenInclude.
            
            // Let's modify the generic repository to support ThenInclude or just use string includes?
            // "params Expression<Func<T, object>>[] includes" supports "x => x.ProductVariant".
            // It does NOT easily support "x => x.ProductVariant.Product".
            
            // Alternative: Fetch batches with basic generic paging, then fetch variants for those batch IDs.
            
            var pagedResult = await _productBatchRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                null, // No filter for now, or maybe IsActive
                request.SortField,
                request.SortOrder,
                b => b.ProductVariant.Product
            );

            // Map to DTO
            var dtos = new System.Collections.Generic.List<ProductBatchResponseDto>();
            
            foreach (var batch in pagedResult.Items)
            {
                string productName = "Unknown Product";
                string variantSKU = "Unknown SKU";

                if (batch.ProductVariant != null)
                {
                    variantSKU = batch.ProductVariant.SKU;
                    
                    // Optimization: We could have pre-loaded products, but for pageSize=10 doing N+1 is manageable strictly for this task,
                    // BUT better to avoid it.
                    // Since I cannot change Repository easily to generic ThenInclude without more work, 
                    // I will check if ProductVariant has Product loaded.
                    // The generic include `b => b.ProductVariant` only loads the variant.
                    
                    // To get Product Name efficiently without changing Repository logic too much:
                    // We can use the IProductVariantRepository to get details if needed, OR just accept N+1 for now as per instructions "implement service".
                    // Let's try to get the product info via the existing _productVariantRepository helper if useful, or load it here.
                    
                    // Actually, let's just create the response. If Product is null, we might need to fetch it.
                    // Users usually want to see Product Name.
                    
                    // Hack/Optimization: Load all ProductVariants with Products for the batch list.
                    var variantWithProduct = await _productVariantRepository.GetVariantWithProduct(batch.ProductVariantId);
                    if (variantWithProduct != null && variantWithProduct.Product != null)
                    {
                        productName = variantWithProduct.Product.Name;
                    }
                }

                dtos.Add(new ProductBatchResponseDto
                {
                    Id = batch.Id,
                    BatchNumber = batch.BatchNumber,
                    ProductVariantId = batch.ProductVariantId,
                    ProductName = productName,
                    VariantSKU = variantSKU,
                    ManufactureDate = batch.ManufactureDate,
                    ExpiryDate = batch.ExpiryDate,
                    CostPrice = batch.CostPrice,
                    QRCodePath = batch.QRCodePath,
                    PurchaseOrderNumber = batch.PurchaseOrderNumber,
                    Supplier = batch.Supplier,
                    IsActive = batch.IsActive,
                    CreatedDate = batch.CreatedDate
                });
            }

            return new ThuocGiaThat.Infrastucture.Common.PagedResult<ProductBatchResponseDto>(
                dtos,
                pagedResult.TotalCount,
                pagedResult.PageNumber,
                pagedResult.PageSize
            );
        }
    }
}
