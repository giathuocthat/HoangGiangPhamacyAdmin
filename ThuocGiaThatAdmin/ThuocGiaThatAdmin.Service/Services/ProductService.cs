using Azure;
using Microsoft.EntityFrameworkCore;
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
using ThuocGiaThatAdmin.Domain.Enums;
using static ThuocGiaThatAdmin.Domain.Constants.AdminPermission;
using Product = ThuocGiaThatAdmin.Domain.Entities.Product;

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
        public async Task<(IEnumerable<Product> products, int TotalCount)> GetPagedProductsAsync(string? category, string? price, int? type, string? sort, int page = 1, int pageSize = 20)
        {
            if (page <= 0)
                throw new ArgumentException("Page number must be greater than 0", nameof(page));

            if (pageSize <= 0 || pageSize > 100)
                throw new ArgumentException("Page size must be between 1 and 100", nameof(pageSize));

            return await _productRepository.GetPagedProductsAsync(category, price, type, sort, page, pageSize);
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

        /// <summary>
        /// Create a new product from DTO with complete mapping and response
        /// </summary>
        /// <param name="dto">Product creation data</param>
        /// <returns>Created product with full details</returns>
        public async Task<ProductResponseDto> CreateProductFromDtoAsync(CreateProductDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Map DTO to Product entity
            var product = new Product
            {
                CategoryId = dto.CategoryId,
                BrandId = dto.BrandId,
                Name = dto.Name,
                ShortDescription = dto.ShortDescription,
                FullDescription = dto.FullDescription,
                Slug = dto.Slug,
                ThumbnailUrl = dto.ThumbnailUrl,
                Ingredients = dto.Ingredients,
                UsageInstructions = dto.UsageInstructions,
                Contraindications = dto.Contraindications,
                StorageInstructions = dto.StorageInstructions,
                RegistrationNumber = dto.RegistrationNumber,
                Specification = dto.Specification,
                IsPrescriptionDrug = dto.IsPrescriptionDrug,
                IsActive = dto.IsActive,
                IsFeatured = dto.IsFeatured,
                DrugEfficacy = dto.DrugEfficacy,
                DosageInstructions = dto.DosageInstructions,
                Indication = dto.Indication,
                Overdose = dto.Overdose
            };

            // Map Images
            if (dto.Images != null && dto.Images.Any())
            {
                foreach (var imageDto in dto.Images)
                {
                    product.Images.Add(new ProductImage
                    {
                        ImageUrl = imageDto.ImageUrl,
                        AltText = imageDto.AltText,
                        DisplayOrder = imageDto.DisplayOrder
                    });
                }
            }

            // Map Product Variants
            if (dto.ProductVariants != null && dto.ProductVariants.Any())
            {
                foreach (var variantDto in dto.ProductVariants)
                {
                    var variant = new ProductVariant
                    {
                        SKU = variantDto.SKU,
                        Barcode = variantDto.Barcode,
                        Price = variantDto.Price,
                        OriginalPrice = variantDto.OriginalPrice,
                        StockQuantity = variantDto.StockQuantity,
                        MaxSalesQuantity = variantDto.MaxSalesQuantity,
                        Weight = variantDto.Weight,
                        Dimensions = variantDto.Dimensions,
                        ImageUrl = variantDto.ImageUrl,
                        IsActive = variantDto.IsActive
                    };

                    // Map Variant Option Values
                    if (variantDto.VariantOptionValueIds != null && variantDto.VariantOptionValueIds.Any())
                    {
                        foreach (var optionValueId in variantDto.VariantOptionValueIds)
                        {
                            variant.VariantOptionValues.Add(new VariantOptionValue
                            {
                                ProductOptionValueId = optionValueId
                            });
                        }
                    }

                    product.ProductVariants.Add(variant);
                }
            }

            // Map Product Active Ingredients
            if (dto.ProductActiveIngredients != null && dto.ProductActiveIngredients.Any())
            {
                foreach (var ingredientDto in dto.ProductActiveIngredients)
                {
                    product.ProductActiveIngredients.Add(new ProductActiveIngredient
                    {
                        ActiveIngredientId = ingredientDto.ActiveIngredientId ?? 0,
                        Quantity = ingredientDto.Quantity,
                        DisplayOrder = ingredientDto.DisplayOrder,
                        IsMainIngredient = ingredientDto.IsMainIngredient
                    });
                }
            }

            // Create product using existing method
            await CreateProductAsync(product);

            // Retrieve created product with all related data
            var createdProduct = await GetProductByIdAsync(product.Id);

            if (createdProduct == null)
                throw new InvalidOperationException("Failed to retrieve created product");

            // Map to response DTO
            return new ProductResponseDto
            {
                Id = createdProduct.Id,
                CategoryId = createdProduct.CategoryId,
                BrandId = createdProduct.BrandId,
                Name = createdProduct.Name,
                ShortDescription = createdProduct.ShortDescription,
                FullDescription = createdProduct.FullDescription,
                Slug = createdProduct.Slug,
                ThumbnailUrl = createdProduct.ThumbnailUrl,
                Ingredients = createdProduct.Ingredients,
                UsageInstructions = createdProduct.UsageInstructions,
                Contraindications = createdProduct.Contraindications,
                StorageInstructions = createdProduct.StorageInstructions,
                RegistrationNumber = createdProduct.RegistrationNumber,
                IsPrescriptionDrug = createdProduct.IsPrescriptionDrug,
                IsActive = createdProduct.IsActive,
                IsFeatured = createdProduct.IsFeatured,
                CreatedDate = createdProduct.CreatedDate,
                UpdatedDate = createdProduct.UpdatedDate,
                DrugEfficacy = createdProduct.DrugEfficacy,
                DosageInstructions = createdProduct.DosageInstructions,
                BrandName = createdProduct.Brand?.Name,
                CategoryName = createdProduct.Category?.Name,
                Images = createdProduct.Images.Select(i => new ProductImageResponseDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    AltText = i.AltText,
                    DisplayOrder = i.DisplayOrder
                }).ToList(),
                ProductVariants = createdProduct.ProductVariants.Select(v => new ProductVariantResponseDto
                {
                    Id = v.Id,
                    SKU = v.SKU,
                    Barcode = v.Barcode,
                    Price = v.Price,
                    OriginalPrice = v.OriginalPrice,
                    StockQuantity = v.StockQuantity,
                    MaxSalesQuantity = v.MaxSalesQuantity,
                    Weight = v.Weight,
                    Dimensions = v.Dimensions,
                    ImageUrl = v.ImageUrl,
                    IsActive = v.IsActive,
                    OptionValues = v.VariantOptionValues.Select(vov => new VariantOptionValueResponseDto
                    {
                        OptionValueId = vov.ProductOptionValueId,
                        OptionValue = vov.ProductOptionValue.Value,
                        OptionId = vov.ProductOptionValue.ProductOption.Id,
                        OptionName = vov.ProductOptionValue.ProductOption.Name
                    }).ToList()
                }).ToList()
            };
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

        /// <summary>
        /// Update an existing product using DTO
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="dto">Product update data</param>
        /// <returns>Number of affected records</returns>
        public async Task<int> UpdateProductAsync(int id, CreateProductDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (id <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(id));

            // Get existing product with all related data
            var existingProduct = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.ProductVariants)
                .ThenInclude(v => v.VariantOptionValues)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProduct == null)
                throw new InvalidOperationException($"Product with ID {id} not found");

            // Update basic properties
            existingProduct.CategoryId = dto.CategoryId;
            existingProduct.BrandId = dto.BrandId;
            existingProduct.Name = dto.Name;
            existingProduct.ShortDescription = dto.ShortDescription;
            existingProduct.FullDescription = dto.FullDescription;
            existingProduct.Slug = dto.Slug;
            existingProduct.ThumbnailUrl = dto.ThumbnailUrl;
            existingProduct.Ingredients = dto.Ingredients;
            existingProduct.UsageInstructions = dto.UsageInstructions;
            existingProduct.Contraindications = dto.Contraindications;
            existingProduct.StorageInstructions = dto.StorageInstructions;
            existingProduct.RegistrationNumber = dto.RegistrationNumber;
            existingProduct.Specification = dto.Specification;
            existingProduct.IsPrescriptionDrug = dto.IsPrescriptionDrug;
            existingProduct.IsActive = dto.IsActive;
            existingProduct.IsFeatured = dto.IsFeatured;
            existingProduct.DrugEfficacy = dto.DrugEfficacy;
            existingProduct.DosageInstructions = dto.DosageInstructions;
            existingProduct.Indication = dto.Indication;
            existingProduct.Overdose = dto.Overdose;

            // Update Images - Smart update logic
            if (dto.Images != null && dto.Images.Any())
            {
                // Get existing image URLs
                var existingImageUrls = existingProduct.Images
                    .Select(img => img.ImageUrl)
                    .ToList();

                // Get DTO image URLs
                var dtoImageUrls = dto.Images
                    .Select(img => img.ImageUrl)
                    .ToList();

                // Remove images not in DTO
                var imagesToRemove = existingProduct.Images
                    .Where(img => !dtoImageUrls.Contains(img.ImageUrl))
                    .ToList();

                foreach (var image in imagesToRemove)
                {
                    existingProduct.Images.Remove(image);
                }

                // Update existing images or add new ones
                foreach (var imageDto in dto.Images)
                {
                    var existingImage = existingProduct.Images
                        .FirstOrDefault(img => img.ImageUrl == imageDto.ImageUrl);

                    if (existingImage != null)
                    {
                        // Update existing image
                        existingImage.AltText = imageDto.AltText;
                        existingImage.DisplayOrder = imageDto.DisplayOrder;
                    }
                    else
                    {
                        // Add new image
                        existingProduct.Images.Add(new ProductImage
                        {
                            ImageUrl = imageDto.ImageUrl,
                            AltText = imageDto.AltText,
                            DisplayOrder = imageDto.DisplayOrder
                        });
                    }
                }
            }
            else
            {
                // If no images in DTO, clear all
                existingProduct.Images.Clear();
            }

            // Update Product Variants - Smart update logic
            if (dto.ProductVariants != null && dto.ProductVariants.Any())
            {
                // Get IDs of variants in the DTO
                var dtoVariantIds = dto.ProductVariants
                    .Where(v => v.Id > 0)
                    .Select(v => v.Id)
                    .ToList();

                // Inactive variants that are not in the DTO
                foreach (var existingVariant in existingProduct.ProductVariants)
                {
                    if (!dtoVariantIds.Contains(existingVariant.Id))
                    {
                        existingVariant.IsActive = false;
                    }
                }

                // Process each variant from DTO
                foreach (var variantDto in dto.ProductVariants)
                {
                    if (variantDto.Id == 0)
                    {
                        // Create new variant
                        var newVariant = new ProductVariant
                        {
                            ProductId = id,
                            SKU = variantDto.SKU,
                            Barcode = variantDto.Barcode,
                            Price = variantDto.Price,
                            OriginalPrice = variantDto.OriginalPrice,
                            StockQuantity = variantDto.StockQuantity,
                            MaxSalesQuantity = variantDto.MaxSalesQuantity,
                            Weight = variantDto.Weight,
                            Dimensions = variantDto.Dimensions,
                            ImageUrl = variantDto.ImageUrl,
                            IsActive = variantDto.IsActive
                        };

                        // Map Variant Option Values
                        if (variantDto.VariantOptionValueIds != null && variantDto.VariantOptionValueIds.Any())
                        {
                            foreach (var optionValueId in variantDto.VariantOptionValueIds)
                            {
                                newVariant.VariantOptionValues.Add(new VariantOptionValue
                                {
                                    ProductOptionValueId = optionValueId
                                });
                            }
                        }

                        existingProduct.ProductVariants.Add(newVariant);
                    }
                    else
                    {
                        // Update existing variant
                        var existingVariant = existingProduct.ProductVariants
                            .FirstOrDefault(v => v.Id == variantDto.Id);

                        if (existingVariant != null)
                        {
                            existingVariant.SKU = variantDto.SKU;
                            existingVariant.Barcode = variantDto.Barcode;
                            existingVariant.Price = variantDto.Price;
                            existingVariant.OriginalPrice = variantDto.OriginalPrice;
                            existingVariant.StockQuantity = variantDto.StockQuantity;
                            existingVariant.MaxSalesQuantity = variantDto.MaxSalesQuantity;
                            existingVariant.Weight = variantDto.Weight;
                            existingVariant.Dimensions = variantDto.Dimensions;
                            existingVariant.ImageUrl = variantDto.ImageUrl;
                            existingVariant.IsActive = variantDto.IsActive;

                            // Update Variant Option Values - Smart update logic
                            if (variantDto.VariantOptionValueIds != null && variantDto.VariantOptionValueIds.Any())
                            {
                                // Get existing option value IDs
                                var existingOptionValueIds = existingVariant.VariantOptionValues
                                    .Select(vov => vov.ProductOptionValueId)
                                    .ToList();

                                // Remove option values not in DTO
                                var optionValuesToRemove = existingVariant.VariantOptionValues
                                    .Where(vov => !variantDto.VariantOptionValueIds.Contains(vov.ProductOptionValueId))
                                    .ToList();

                                foreach (var optionValue in optionValuesToRemove)
                                {
                                    existingVariant.VariantOptionValues.Remove(optionValue);
                                }

                                // Add new option values from DTO
                                foreach (var optionValueId in variantDto.VariantOptionValueIds)
                                {
                                    if (!existingOptionValueIds.Contains(optionValueId))
                                    {
                                        existingVariant.VariantOptionValues.Add(new VariantOptionValue
                                        {
                                            ProductOptionValueId = optionValueId
                                        });
                                    }
                                }
                            }
                            else
                            {
                                // If no option values in DTO, clear all
                                existingVariant.VariantOptionValues.Clear();
                            }
                        }
                    }
                }
            }
            else
            {
                // If no variants in DTO, inactive all existing variants
                foreach (var existingVariant in existingProduct.ProductVariants)
                {
                    existingVariant.IsActive = false;
                }
            }

            // Update Product Active Ingredients - Smart update logic
            if (dto.ProductActiveIngredients != null && dto.ProductActiveIngredients.Any())
            {
                // Get all existing ingredients for this product
                var existingIngredients = await _context.ProductActiveIngredients
                    .Where(pai => pai.ProductId == id)
                    .ToListAsync();

                // Get IDs of ingredients in the DTO
                var dtoIngredientIds = dto.ProductActiveIngredients
                    .Where(i => i.Id.HasValue && i.Id.Value > 0)
                    .Select(i => i.Id.Value)
                    .ToList();

                // Remove ingredients that are not in the DTO
                var ingredientsToRemove = existingIngredients
                    .Where(ei => !dtoIngredientIds.Contains(ei.Id))
                    .ToList();

                _context.ProductActiveIngredients.RemoveRange(ingredientsToRemove);

                // Process each ingredient from DTO
                foreach (var ingredientDto in dto.ProductActiveIngredients)
                {
                    if (!ingredientDto.Id.HasValue || ingredientDto.Id.Value == 0)
                    {
                        // Create new ingredient
                        _context.ProductActiveIngredients.Add(new ProductActiveIngredient
                        {
                            ProductId = id,
                            ActiveIngredientId = ingredientDto.ActiveIngredientId ?? 0,
                            Quantity = ingredientDto.Quantity,
                            DisplayOrder = ingredientDto.DisplayOrder,
                            IsMainIngredient = ingredientDto.IsMainIngredient
                        });
                    }
                    else
                    {
                        // Update existing ingredient
                        var existingIngredient = existingIngredients
                            .FirstOrDefault(ei => ei.Id == ingredientDto.Id.Value);

                        if (existingIngredient != null)
                        {
                            existingIngredient.ActiveIngredientId = ingredientDto.ActiveIngredientId ?? 0;
                            existingIngredient.Quantity = ingredientDto.Quantity;
                            existingIngredient.DisplayOrder = ingredientDto.DisplayOrder;
                            existingIngredient.IsMainIngredient = ingredientDto.IsMainIngredient;
                        }
                    }
                }
            }
            else
            {
                // If no ingredients in DTO, remove all existing ingredients
                var existingIngredients = _context.ProductActiveIngredients
                    .Where(pai => pai.ProductId == id)
                    .ToList();
                _context.ProductActiveIngredients.RemoveRange(existingIngredients);
            }

            return await _context.SaveChangesAsync();
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
        public async Task<(IEnumerable<dynamic> products, int totalCount)> GetStoreProductsAsync(int pageNumber = 1,
            int pageSize = 10)
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
                .Include(p => p.ProductVariants)
                .ThenInclude(v => v.VariantOptionValues)
                .ThenInclude(vov => vov.ProductOptionValue)
                .ThenInclude(pov => pov.ProductOption)
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
                product.Specification,
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
                        : Enumerable.Empty<object>().Select(x => new
                            { StatusType = default(ProductStatusType), StatusName = string.Empty }).ToList(),

                    // Option values (e.g., Màu đen, Size X)
                    OptionValues = variant.VariantOptionValues.Select(vov => new
                    {
                        OptionId = vov.ProductOptionValue.ProductOption.Id,
                        OptionName = vov.ProductOptionValue.ProductOption.Name,
                        OptionValueId = vov.ProductOptionValueId,
                        OptionValue = vov.ProductOptionValue.Value
                    }).ToList()
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
                throw new InvalidOperationException(
                    $"ProductStatusMap already exists for ProductVariantId {dto.ProductVariantId} with StatusType {dto.StatusType}");

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

        public async Task<IEnumerable<dynamic>> GetProductCollectionByTypeAsync(ProductStatusType productStatusType,
            int pageSize = 10)
        {
            var productVariantIds = _context.ProductStatusMaps.Where(x => x.StatusType == productStatusType)
                .Take(pageSize).Select(x => x.ProductVariantId).ToList();
            return await _context.ProductVariants.Where(x => productVariantIds.Contains(x.Id)).Select(x => new
            {
                Id = x.ProductId,
                ProductId = x.ProductId,
                ProductVariantId = x.Id,
                Price = x.Price,
                OriginalPrice = x.OriginalPrice,
                Name = x.Product.Name,
                MaxSalesQuantity = x.MaxSalesQuantity,
                thumbnailUrl = x.ImageUrl,
                Slug = x.Product.Slug
            }).ToListAsync();
        }

        /// <summary>
        /// Search products by name or SKU for AutoComplete
        /// </summary>
        /// <param name="keyword">Search keyword (name or SKU)</param>
        /// <param name="limit">Max results to return</param>
        /// <returns>List of products with variant info</returns>
        public async Task<IEnumerable<dynamic>> SearchProductsForSelectionAsync(string keyword, int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return Enumerable.Empty<dynamic>();

            keyword = keyword.Trim().ToLower();

            var results = await _context.Products
                .Where(p => p.IsActive)
                .Where(p => p.Name.ToLower().Contains(keyword) ||
                            p.ProductVariants.Any(v => v.SKU.ToLower().Contains(keyword)))
                .Include(p => p.ProductVariants.Where(v => v.IsActive))
                .Take(limit)
                .AsNoTracking()
                .ToListAsync();

            return results.Select(p =>
            {
                var firstVariant = p.ProductVariants.FirstOrDefault();
                return new
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    ThumbnailUrl = p.ThumbnailUrl,
                    Specification = p.Specification,
                    ProductVariantId = firstVariant?.Id,
                    SKU = firstVariant?.SKU,
                    Price = firstVariant?.Price,
                    OriginalPrice = firstVariant?.OriginalPrice,
                    ImageUrl = firstVariant?.ImageUrl ?? p.ThumbnailUrl
                };
            });
        }

        /// <summary>
        /// Get product variants with filtering, searching, and pagination
        /// </summary>
        /// <param name="request">Request parameters for filtering and pagination</param>
        /// <returns>Tuple containing list of product variants and total count</returns>
        public async Task<(IEnumerable<ProductVariantListItemDto> variants, int totalCount)> GetProductVariantsAsync(
            GetProductVariantsRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.PageNumber <= 0)
                throw new ArgumentException("Page number must be greater than 0", nameof(request.PageNumber));

            if (request.PageSize <= 0 || request.PageSize > 100)
                throw new ArgumentException("Page size must be between 1 and 100", nameof(request.PageSize));

            // Build query
            var query = _context.ProductVariants
                .Include(v => v.Product)
                .Include(v => v.VariantOptionValues)
                .ThenInclude(vov => vov.ProductOptionValue)
                .ThenInclude(pov => pov.ProductOption)
                .AsQueryable();

            // Apply filters
            if (request.IsProductActive.HasValue)
            {
                query = query.Where(v => v.Product.IsActive == request.IsProductActive.Value);
            }
            else
            {
                query = query.Where(v => v.Product.IsActive == true);
            }

            if (request.IsVariantActive.HasValue)
            {
                query = query.Where(v => v.IsActive == request.IsVariantActive.Value);
            }
            else
            {
                query = query.Where(v => v.IsActive == true);
            }

            // Apply search
            if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
            {
                var keyword = request.SearchKeyword.Trim().ToLower();
                query = query.Where(v =>
                    v.Product.Name.Contains(keyword) ||
                    v.SKU.Contains(keyword));
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination and get results
            var variants = await query
                .OrderByDescending(v => v.CreatedDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .AsNoTracking()
                .ToListAsync();

            // Map to DTOs
            var result = variants.Select(v => new ProductVariantListItemDto
            {
                VariantId = v.Id,
                ProductId = v.ProductId,
                ProductName = v.Product.Name,
                SKU = v.SKU,
                Barcode = v.Barcode,
                Price = v.Price,
                OriginalPrice = v.OriginalPrice,
                StockQuantity = v.StockQuantity,
                MaxSalesQuantity = v.MaxSalesQuantity,
                ImageUrl = v.ImageUrl,
                IsProductActive = v.Product.IsActive,
                IsVariantActive = v.IsActive,
                OptionValues = v.VariantOptionValues.Select(vov => new VariantOptionInfo
                {
                    OptionId = vov.ProductOptionValue.ProductOption.Id,
                    OptionName = vov.ProductOptionValue.ProductOption.Name,
                    OptionValueId = vov.ProductOptionValueId,
                    OptionValue = vov.ProductOptionValue.Value
                }).ToList()
            }).ToList();

            return (result, totalCount);
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        public async Task<Product?> GetProductBySlugAsync(string slug)
        {
            if(string.IsNullOrEmpty(slug)) throw  new ArgumentNullException("slug");

            return await _productRepository.GetProductWithCategoryAsync(slug);
        }
    }
}
