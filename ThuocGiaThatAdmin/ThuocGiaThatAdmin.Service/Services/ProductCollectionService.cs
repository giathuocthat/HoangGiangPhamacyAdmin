using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThat.Infrastucture;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class ProductCollectionService : IProductCollectionService
    {
        private readonly TrueMecContext _context;
        private readonly IProductCollectionRepository _collectionRepository;
        private readonly IProductMaxOrderConfigRepository _maxOrderConfigRepository;

        public ProductCollectionService(
            TrueMecContext context,
            IProductCollectionRepository collectionRepository,
            IProductMaxOrderConfigRepository maxOrderConfigRepository)
        {
            _context = context;
            _collectionRepository = collectionRepository;
            _maxOrderConfigRepository = maxOrderConfigRepository;
        }

      

        public async Task<List<ProductDto>> GetNewProductsAsync(int days = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            
            var products = await _context.Products
                .Where(p => p.IsActive)
                .Where(p => p.CreatedDate >= cutoffDate)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .Include(p => p.ProductVariants)
                .Include(p => p.MaxOrderConfig)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            return products.Select(MapToProductDto).ToList();
        }

        public async Task<List<ProductDto>> GetLowStockProductsAsync(int maxStock = 100)
        {
            // Get products where total stock of all variants is less than maxStock
            var products = await _context.Products
                .Where(p => p.IsActive)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .Include(p => p.ProductVariants)
                .Include(p => p.MaxOrderConfig)
                .Where(p => p.ProductVariants.Sum(v => v.StockQuantity) < maxStock)
                .OrderBy(p => p.ProductVariants.Sum(v => v.StockQuantity))
                .ToListAsync();

            return products.Select(MapToProductDto).ToList();
        }

        public async Task<ProductCollectionDto> CreateCollectionAsync(CreateCollectionDto dto)
        {
            var collection = new ProductCollection
            {
                Name = dto.Name,
                Slug = GenerateSlug(dto.Name),
                Description = dto.Description,
                Type = dto.Type,
                DisplayOrder = dto.DisplayOrder,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            await _collectionRepository.AddAsync(collection);
            await _context.SaveChangesAsync();

            return MapToCollectionDto(collection);
        }

        public async Task AddProductsToCollectionAsync(int collectionId, List<int> productIds)
        {
            int currentMaxOrder = 0;
            var collection = await _collectionRepository.GetWithProductsAsync(collectionId);
            if (collection != null && collection.Items.Any())
            {
                currentMaxOrder = collection.Items.Max(i => i.DisplayOrder);
            }

            foreach (var productId in productIds)
            {
                // Check if already exists
                var exists = collection?.Items.Any(i => i.ProductId == productId) ?? false;
                if (!exists)
                {
                    currentMaxOrder++;
                    await _collectionRepository.AddProductToCollectionAsync(collectionId, productId, currentMaxOrder);
                }
            }
        }

        // Max order config

        public async Task<List<ProductDto>> GetProductsWithMaxOrderAsync()
        {
            var configs = await _maxOrderConfigRepository.GetActiveConfigsAsync();
            var products = configs.Select(c => c.Product).Where(p => p.IsActive).ToList();
            
            // Need to load full details for these products
            var productIds = products.Select(p => p.Id).ToList();
            var fullProducts = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .Include(p => p.ProductVariants)
                .Include(p => p.MaxOrderConfig)
                .ToListAsync();

            return fullProducts.Select(MapToProductDto).ToList();
        }

        public async Task SetMaxOrderConfigAsync(int productId, SetMaxOrderDto dto)
        {
            var config = await _maxOrderConfigRepository.GetByProductIdAsync(productId);
            
            if (config == null)
            {
                config = new ProductMaxOrderConfig
                {
                    ProductId = productId,
                    MaxQuantityPerOrder = dto.MaxQuantityPerOrder,
                    MaxQuantityPerDay = dto.MaxQuantityPerDay,
                    MaxQuantityPerMonth = dto.MaxQuantityPerMonth,
                    Reason = dto.Reason,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };
                await _maxOrderConfigRepository.AddAsync(config);
            }
            else
            {
                config.MaxQuantityPerOrder = dto.MaxQuantityPerOrder;
                config.MaxQuantityPerDay = dto.MaxQuantityPerDay;
                config.MaxQuantityPerMonth = dto.MaxQuantityPerMonth;
                config.Reason = dto.Reason;
                config.IsActive = true;
                config.UpdatedDate = DateTime.UtcNow;
                await _maxOrderConfigRepository.UpdateAsync(config);
            }
            
            await _context.SaveChangesAsync();
        }

        // Helpers

        private ProductDto MapToProductDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty,
                BrandId = product.BrandId,
                BrandName = product.Brand?.Name,
                Name = product.Name,
                ShortDescription = product.ShortDescription,
                FullDescription = product.FullDescription,
                Slug = product.Slug,
                ThumbnailUrl = product.ThumbnailUrl,
                Ingredients = product.Ingredients,
                UsageInstructions = product.UsageInstructions,
                Contraindications = product.Contraindications,
                StorageInstructions = product.StorageInstructions,
                RegistrationNumber = product.RegistrationNumber,
                IsPrescriptionDrug = product.IsPrescriptionDrug,
                IsActive = product.IsActive,
                IsFeatured = product.IsFeatured,
                CreatedDate = product.CreatedDate,
                Images = product.Images.Select(i => new ProductImageDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    //IsPrimary = i.IsPrimary,
                    DisplayOrder = i.DisplayOrder
                }).ToList(),
                ProductVariants = product.ProductVariants.Select(v => new ProductVariantDto
                {
                    Id = v.Id,
                    //ProductId = v.ProductId,
                    SKU = v.SKU,
                    Barcode = v.Barcode,
                    Price = v.Price,
                    OriginalPrice = v.OriginalPrice,
                    StockQuantity = v.StockQuantity,
                    Weight = v.Weight,
                    Dimensions = v.Dimensions,
                    ImageUrl = v.ImageUrl,
                    IsActive = v.IsActive
                }).ToList(),
                MaxOrderConfig = product.MaxOrderConfig != null ? new ProductMaxOrderConfigDto
                {
                    MaxQuantityPerOrder = product.MaxOrderConfig.MaxQuantityPerOrder,
                    MaxQuantityPerDay = product.MaxOrderConfig.MaxQuantityPerDay,
                    MaxQuantityPerMonth = product.MaxOrderConfig.MaxQuantityPerMonth,
                    Reason = product.MaxOrderConfig.Reason
                } : null
            };
        }

        private ProductCollectionDto MapToCollectionDto(ProductCollection collection)
        {
            return new ProductCollectionDto
            {
                Id = collection.Id,
                Name = collection.Name,
                Slug = collection.Slug,
                Description = collection.Description,
                Type = collection.Type,
                IsActive = collection.IsActive,
                DisplayOrder = collection.DisplayOrder,
                StartDate = collection.StartDate,
                EndDate = collection.EndDate
            };
        }

        private string GenerateSlug(string name)
        {
            // Simple slug generation
            return name.ToLower()
                .Replace(" ", "-")
                .Replace("đ", "d")
                .Replace("á", "a").Replace("à", "a").Replace("ả", "a").Replace("ã", "a").Replace("ạ", "a")
                .Replace("ă", "a").Replace("ắ", "a").Replace("ằ", "a").Replace("ẳ", "a").Replace("ẵ", "a").Replace("ặ", "a")
                .Replace("â", "a").Replace("ấ", "a").Replace("ầ", "a").Replace("ẩ", "a").Replace("ẫ", "a").Replace("ậ", "a")
                .Replace("é", "e").Replace("è", "e").Replace("ẻ", "e").Replace("ẽ", "e").Replace("ẹ", "e")
                .Replace("ê", "e").Replace("ế", "e").Replace("ề", "e").Replace("ể", "e").Replace("ễ", "e").Replace("ệ", "e")
                .Replace("í", "i").Replace("ì", "i").Replace("ỉ", "i").Replace("ĩ", "i").Replace("ị", "i")
                .Replace("ó", "o").Replace("ò", "o").Replace("ỏ", "o").Replace("õ", "o").Replace("ọ", "o")
                .Replace("ô", "o").Replace("ố", "o").Replace("ồ", "o").Replace("ổ", "o").Replace("ỗ", "o").Replace("ộ", "o")
                .Replace("ơ", "o").Replace("ớ", "o").Replace("ờ", "o").Replace("ở", "o").Replace("ỡ", "o").Replace("ợ", "o")
                .Replace("ú", "u").Replace("ù", "u").Replace("ủ", "u").Replace("ũ", "u").Replace("ụ", "u")
                .Replace("ư", "u").Replace("ứ", "u").Replace("ừ", "u").Replace("ử", "u").Replace("ữ", "u").Replace("ự", "u")
                .Replace("ý", "y").Replace("ỳ", "y").Replace("ỷ", "y").Replace("ỹ", "y").Replace("ỵ", "y");
        }

        public Task<List<ProductDto>> GetHighProfitProductsAsync(decimal minRevenue = 100000000)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductDto>> GetTopSellingProductsAsync(int minQuantity = 100)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductDto>> GetCollectionProductsAsync(string slug)
        {
            throw new NotImplementedException();
        }
    }
}
