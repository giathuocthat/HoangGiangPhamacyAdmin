using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contract.Requests;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Service.Services
{
    /// <summary>
    /// Service for Product business logic
    /// </summary>
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly TrueMecContext _context;

        public ProductService(IProductRepository productRepository, TrueMecContext context)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Read Operations

        /// <summary>
        /// Get all products
        /// </summary>
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllWithBrandAndCategoryAsync();
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(id));

            return await _productRepository.GetProductWithCategoryAsync(id);
        }

        /// <summary>
        /// Get products by category ID
        /// </summary>
        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Category ID must be greater than 0", nameof(categoryId));

            return await _productRepository.GetByCategoryIdAsync(categoryId);
        }

        /// <summary>
        /// Search products by name
        /// </summary>
        public async Task<IEnumerable<Product>> SearchProductsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be null or empty", nameof(name));

            return await _productRepository.GetByNameAsync(name);
        }

        /// <summary>
        /// Get products with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Tuple containing list of products and total count</returns>
        public async Task<(IEnumerable<Product> products, int TotalCount)> GetPagedProductsAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0)
                throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

            if (pageSize <= 0 || pageSize > 100)
                throw new ArgumentException("Page size must be between 1 and 100", nameof(pageSize));

            return await _productRepository.GetPagedProductsAsync(pageNumber, pageSize);
        }

        #endregion

        #region Create Operation

        /// <summary>
        /// Create a new product
        /// </summary>
        public async Task<int> CreateProductAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product name cannot be null or empty", nameof(product.Name));

            if (product.CategoryId <= 0)
                throw new ArgumentException("Category ID must be greater than 0", nameof(product.CategoryId));

            await _productRepository.AddAsync(product);
            return await _productRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Create multiple products
        /// </summary>
        public async Task<int> CreateProductsAsync(IEnumerable<Product> products)
        {
            if (products == null || !products.Any())
                throw new ArgumentException("Products list cannot be null or empty", nameof(products));

            foreach (var product in products)
            {
                if (string.IsNullOrWhiteSpace(product.Name))
                    throw new ArgumentException("Product name cannot be null or empty", nameof(product.Name));

                if (product.CategoryId <= 0)
                    throw new ArgumentException("Category ID must be greater than 0", nameof(product.CategoryId));
            }

            await _productRepository.AddRangeAsync(products);
            return await _productRepository.SaveChangesAsync();
        }

        #endregion

        #region Update Operation

        /// <summary>
        /// Update an existing product
        /// </summary>
        public async Task<int> UpdateProductAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (product.Id <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(product.Id));

            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product name cannot be null or empty", nameof(product.Name));

            if (product.CategoryId <= 0)
                throw new ArgumentException("Category ID must be greater than 0", nameof(product.CategoryId));

            // Check if product exists
            var existingProduct = await _productRepository.GetByIdAsync(product.Id);
            if (existingProduct == null)
                throw new InvalidOperationException($"Product with ID {product.Id} not found");

            _productRepository.Update(product);
            return await _productRepository.SaveChangesAsync();
        }

        #endregion

        #region Delete Operation

        /// <summary>
        /// Delete a product by ID
        /// </summary>
        public async Task<int> DeleteProductAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(id));

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new InvalidOperationException($"Product with ID {id} not found");

            _productRepository.Delete(product);
            return await _productRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Delete multiple products
        /// </summary>
        public async Task<int> DeleteProductsAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
                throw new ArgumentException("Product IDs list cannot be null or empty", nameof(ids));

            var products = new List<Product>();
            foreach (var id in ids)
            {
                if (id <= 0)
                    throw new ArgumentException($"Product ID must be greater than 0, got: {id}", nameof(ids));

                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                    throw new InvalidOperationException($"Product with ID {id} not found");

                products.Add(product);
            }

            _productRepository.DeleteRange(products);
            return await _productRepository.SaveChangesAsync();
        }

        #endregion

        #region Store Product Operations

        /// <summary>
        /// Get products for store with detailed inventory and sales information
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Tuple containing products with enhanced data and total count</returns>
        public async Task<(IEnumerable<dynamic> products, int totalCount)> GetStoreProductsAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0)
                throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

            if (pageSize <= 0 || pageSize > 100)
                throw new ArgumentException("Page size must be between 1 and 100", nameof(pageSize));

            // Get total count
            var totalCount = await _context.Products.Where(p => p.IsActive).CountAsync();
            
            // Get products with all related data using efficient eager loading
            var products = await _context.Products
                .Where(p => p.IsActive)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.Inventories)
                .OrderByDescending(p => p.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
            
            // Get all variant IDs for this page
            var variantIds = products
                .SelectMany(p => p.ProductVariants.Select(v => v.Id))
                .ToList();
            
            // Get sold quantities for all variants in one query
            var soldQuantities = await _context.OrderItems
                .Where(oi => variantIds.Contains(oi.ProductVariantId))
                .GroupBy(oi => oi.ProductVariantId)
                .Select(g => new { VariantId = g.Key, SoldQuantity = g.Sum(oi => oi.Quantity) })
                .ToDictionaryAsync(x => x.VariantId, x => x.SoldQuantity);
            
            // Get ProductStatusMaps for all variants in one query
            var productStatusMaps = await _context.ProductStatusMaps
                .Where(psm => variantIds.Contains(psm.ProductVariantId))
                .ToListAsync();
            
            // Group status maps by variant ID
            var statusMapsByVariant = productStatusMaps
                .GroupBy(psm => psm.ProductVariantId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(psm => new { psm.StatusType, psm.StatusName }).ToList()
                );
            
            // Map to dynamic result
            var result = products.Select(product => new
            {
                product.Id,
                product.CategoryId,
                product.BrandId,
                product.Name,
                product.ShortDescription,
                product.Slug,
                product.ThumbnailUrl,
                product.Ingredients,
                product.UsageInstructions,
                product.Contraindications,
                product.StorageInstructions,
                product.RegistrationNumber,
                product.IsPrescriptionDrug,
                product.IsActive,
                product.IsFeatured,
                product.CreatedDate,
                product.UpdatedDate,
                BrandName = product.Brand?.Name,
                CategoryName = product.Category?.Name,
                
                // Enhanced variant information
                ProductVariants = product.ProductVariants.Select(variant => new
                {
                    variant.Id,
                    variant.SKU,
                    variant.Barcode,
                    variant.Price,
                    variant.OriginalPrice,
                    variant.StockQuantity,
                    variant.MaxSalesQuantity,
                    
                    // Aggregate inventory stock from all warehouses
                    InventoryStock = variant.Inventories.Sum(inv => inv.QuantityOnHand),
                    
                    // Get sold quantity from pre-loaded dictionary
                    SoldQuantity = soldQuantities.ContainsKey(variant.Id) ? soldQuantities[variant.Id] : 0,
                    
                    // Get ProductVariantStatuses from pre-loaded dictionary
                    ProductVariantStatuses = statusMapsByVariant.ContainsKey(variant.Id) 
                        ? statusMapsByVariant[variant.Id] 
                        : Enumerable.Empty<object>().Select(x => new { StatusType = default(ProductStatusType), StatusName = string.Empty }).ToList()
                }).ToList()
            });

            return (result, totalCount);
        }


        #endregion

        #region ProductStatusMap Operations

        /// <summary>
        /// Create a new ProductStatusMap
        /// </summary>
        /// <param name="dto">ProductStatusMap creation data</param>
        /// <returns>Created ProductStatusMap</returns>
        public async Task<ProductStatusMapResponseDto> CreateProductStatusMapAsync(CreateProductStatusMapDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.ProductVariantId <= 0)
                throw new ArgumentException("ProductVariantId must be greater than 0", nameof(dto.ProductVariantId));

            // Validate ProductVariant exists
            var variantExists = await _context.ProductVariants
                .AnyAsync(v => v.Id == dto.ProductVariantId);

            if (!variantExists)
                throw new InvalidOperationException($"ProductVariant with ID {dto.ProductVariantId} not found");

            // Check for duplicate (ProductVariantId + StatusType)
            var duplicate = await _context.ProductStatusMaps
                .AnyAsync(psm => psm.ProductVariantId == dto.ProductVariantId && psm.StatusType == dto.StatusType);

            if (duplicate)
                throw new InvalidOperationException($"ProductStatusMap already exists for ProductVariantId {dto.ProductVariantId} with StatusType {dto.StatusType}");

            // Create new ProductStatusMap
            var productStatusMap = new ProductStatusMap
            {
                ProductVariantId = dto.ProductVariantId,
                StatusType = dto.StatusType,
                StatusName = dto.StatusType.ToString()
            };

            _context.ProductStatusMaps.Add(productStatusMap);
            await _context.SaveChangesAsync();

            // Return response DTO
            return new ProductStatusMapResponseDto
            {
                ProductVariantId = productStatusMap.ProductVariantId,
                StatusType = productStatusMap.StatusType,
                StatusName = productStatusMap.StatusName
            };
        }

        #endregion

        
    }
}
