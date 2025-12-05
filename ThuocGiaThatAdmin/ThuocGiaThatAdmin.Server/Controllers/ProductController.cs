using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ThuocGiaThatAdmin.Service.Services;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Server.Models;
using ThuocGiaThatAdmin.Contract.Requests;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    public class ProductController : BaseApiController
    {
        private readonly ProductService _productService;
        private readonly ICartService _cartService;

        public ProductController(ProductService productService, ILogger<ProductController> logger, ICartService cartService)
            : base(logger)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="dto">Product creation data</param>
        /// <returns>Created product</returns>
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
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
                    IsPrescriptionDrug = dto.IsPrescriptionDrug,
                    IsActive = dto.IsActive,
                    IsFeatured = dto.IsFeatured
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

                // Create product using service
                await _productService.CreateProductAsync(product);

                // Return created product with details
                var createdProduct = await _productService.GetProductByIdAsync(product.Id);
                
                var response = new
                {
                    createdProduct.Id,
                    createdProduct.CategoryId,
                    createdProduct.BrandId,
                    createdProduct.Name,
                    createdProduct.ShortDescription,
                    createdProduct.FullDescription,
                    createdProduct.Slug,
                    createdProduct.ThumbnailUrl,
                    createdProduct.Ingredients,
                    createdProduct.UsageInstructions,
                    createdProduct.Contraindications,
                    createdProduct.StorageInstructions,
                    createdProduct.RegistrationNumber,
                    createdProduct.IsPrescriptionDrug,
                    createdProduct.IsActive,
                    createdProduct.IsFeatured,
                    createdProduct.CreatedDate,
                    createdProduct.UpdatedDate,
                    BrandName = createdProduct.Brand?.Name,
                    CategoryName = createdProduct.Category?.Name,
                    Images = createdProduct.Images.Select(i => new
                    {
                        i.Id,
                        i.ImageUrl,
                        i.AltText,
                        i.DisplayOrder
                    }),
                    ProductVariants = createdProduct.ProductVariants.Select(v => new
                    {
                        v.Id,
                        v.SKU,
                        v.Barcode,
                        v.Price,
                        v.OriginalPrice,
                        v.StockQuantity,
                        v.Weight,
                        v.Dimensions,
                        v.ImageUrl,
                        v.IsActive,
                        OptionValues = v.VariantOptionValues.Select(vov => new
                        {
                            OptionValueId = vov.ProductOptionValueId,
                            OptionValue = vov.ProductOptionValue.Value,
                            OptionId = vov.ProductOptionValue.ProductOption.Id,
                            OptionName = vov.ProductOptionValue.ProductOption.Name
                        })
                    })
                };

                return Created(response, "Product created successfully");
            }, "Create Product");
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>List of all products</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();

                var response = products.Select(product => new
                {
                    product.Id,
                    product.CategoryId,
                    product.BrandId,
                    product.Name,
                    product.ShortDescription,
                    product.FullDescription,
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
                    Brand = new {
                        product.Brand?.Id,
                        product.Brand?.Name,
                        product.Brand?.Slug,
                        product.Brand?.CountryOfOrigin,
                        product.Brand?.LogoUrl
                    },
                    Category = new {
                        product.Category.Id,
                        product.Category.Name,
                        product.Category.Slug,
                        product.Category.ParentId
                    },
                    ProductVariants = product.ProductVariants.Select(v => new
                    {
                        v.Id,
                        v.SKU,
                        v.Barcode,
                        v.Price,
                        v.OriginalPrice,
                        v.StockQuantity,
                        v.MaxSalesQuantity,
                        v.Weight,
                        v.Dimensions,
                        v.ImageUrl,
                        v.IsActive
                    }),
                    Images = product.Images.Select(i => new
                    {
                        i.Id,
                        i.ImageUrl,
                        i.AltText,
                        i.DisplayOrder
                    })
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting all products");
                return StatusCode(500, new { message = "An error occurred while retrieving products", error = ex.Message });
            }
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<dynamic>> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound(new { message = $"Product with ID {id} not found" });

                // Shape data specifically for product detail page
                var response = new
                {
                    // Basic info
                    product.Id,
                    product.Name,
                    product.Slug,
                    product.ShortDescription,
                    fullDescription = product.FullDescription?.Replace("\\n",""),
                    product.ThumbnailUrl,
                    // Pharma specifics
                    product.Ingredients,
                    product.UsageInstructions,
                    product.Contraindications,
                    product.StorageInstructions,
                    product.RegistrationNumber,
                    product.IsPrescriptionDrug,

                    // Metadata
                    product.IsActive,
                    product.IsFeatured,
                    product.CreatedDate,
                    product.UpdatedDate,

                    // IDs for editing
                    product.CategoryId,
                    product.BrandId,

                    // Category
                    Category = product.Category == null
                        ? null
                        : new
                        {
                            product.Category.Id,
                            product.Category.Name,
                            product.Category.Slug,
                            product.Category.ParentId
                        },

                    // Brand
                    Brand = product.Brand == null
                        ? null
                        : new
                        {
                            product.Brand.Id,
                            product.Brand.Name,
                            product.Brand.Slug,
                            product.Brand.CountryOfOrigin,
                            product.Brand.LogoUrl
                        },

                    // Images
                    Images = product.Images
                        .OrderBy(i => i.DisplayOrder)
                        .Select(i => new
                        {
                            i.Id,
                            i.ImageUrl,
                            i.DisplayOrder,
                            i.AltText
                        }),

                    // Product options & values
                    ProductOptions = product.ProductOptions
                        .OrderBy(o => o.DisplayOrder)
                        .Select(o => new
                        {
                            o.Id,
                            o.Name,
                            o.DisplayOrder,
                            Values = o.ProductOptionValues
                                .OrderBy(v => v.DisplayOrder)
                                .Select(v => new
                                {
                                    v.Id,
                                    v.Value,
                                    v.DisplayOrder
                                })
                        }),

                    // Variants & their option values
                    ProductVariants = product.ProductVariants
                        .Where(v => v.IsActive)
                        .Select(v => new
                        {
                            v.Id,
                            v.SKU,
                            v.Barcode,
                            v.Price,
                            v.OriginalPrice,
                            v.StockQuantity,
                            v.MaxSalesQuantity,
                            v.Weight,
                            v.Dimensions,
                            v.ImageUrl,
                            v.IsActive,
                            OptionValues = v.VariantOptionValues.Select(vov => new
                            {
                                OptionValueId = vov.ProductOptionValueId,
                                OptionValue = vov.ProductOptionValue.Value,
                                OptionId = vov.ProductOptionValue.ProductOption.Id,
                                OptionName = vov.ProductOptionValue.ProductOption.Name
                            })
                        })
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting product by ID");
                return StatusCode(500, new { message = "An error occurred while retrieving the product", error = ex.Message });
            }
        }

        /// <summary>
        /// Get products by category
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>List of products in the specified category</returns>
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await _productService.GetProductsByCategoryAsync(categoryId);
                return Ok(products);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting products by category");
                return StatusCode(500, new { message = "An error occurred while retrieving products by category", error = ex.Message });
            }
        }

        /// <summary>
        /// Search products by name
        /// </summary>
        /// <param name="name">Product name to search</param>
        /// <returns>List of products matching the search term</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<dynamic>>> SearchProducts([FromQuery] string name)
        {
            try
            {
                var products = await _productService.SearchProductsByNameAsync(name);
                return Ok(products);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error searching products");
                return StatusCode(500, new { message = "An error occurred while searching products", error = ex.Message });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("getPagedProducts")]
        public async Task<ActionResult<dynamic>> GetPagedProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (products, totalCount) = await _productService.GetPagedProductsAsync(pageNumber, pageSize);
                var response = new
                {
                    Data = products.Select(product => new
                    {
                        product.Id,
                        product.CategoryId,
                        product.BrandId,
                        product.Name,
                        product.ShortDescription,
                        product.FullDescription,
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
                        price = product.ProductVariants.FirstOrDefault()?.Price,
                        originalPrice = product.ProductVariants.FirstOrDefault()?.OriginalPrice,
                        productVariantId = product.ProductVariants.FirstOrDefault()?.Id
                    }),
                    Pagination = new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                    }
                };
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting paged products");
                return StatusCode(500, new { message = "An error occurred while retrieving paged products", error = ex.Message });
            }
        }

        /// <summary>
        /// Get paged products with detailed inventory and sales information
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Products with inventory stock, max sales quantity, sold quantity, and ProductVariantStatuses</returns>
        [HttpGet("getStoreProduct")]
        public async Task<ActionResult<dynamic>> GetStoreProduct([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (products, totalCount) = await _productService.GetStoreProductsAsync(pageNumber, pageSize);
                
                var response = new
                {
                    Data = products,
                    Pagination = new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                    }
                };
                
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting store products");
                return StatusCode(500, new { message = "An error occurred while retrieving store products", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="dto">Product update data</param>
        /// <returns>Updated product</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] CreateProductDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                // Get existing product
                var existingProduct = await _productService.GetProductByIdAsync(id);
                if (existingProduct == null)
                    return NotFound(new { message = $"Product with ID {id} not found" });

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
                existingProduct.IsPrescriptionDrug = dto.IsPrescriptionDrug;
                existingProduct.IsActive = dto.IsActive;
                existingProduct.IsFeatured = dto.IsFeatured;

                // Update Images (clear and re-add)
                existingProduct.Images.Clear();
                if (dto.Images != null && dto.Images.Any())
                {
                    foreach (var imageDto in dto.Images)
                    {
                        existingProduct.Images.Add(new ProductImage
                        {
                            ImageUrl = imageDto.ImageUrl,
                            AltText = imageDto.AltText,
                            DisplayOrder = imageDto.DisplayOrder
                        });
                    }
                }

                // Update Product Variants
                existingProduct.ProductVariants.Clear();
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

                        existingProduct.ProductVariants.Add(variant);
                    }
                }

                // Update product using service
                await _productService.UpdateProductAsync(existingProduct);

                // Return updated product with details
                var updatedProduct = await _productService.GetProductByIdAsync(id);

                var response = new
                {
                    updatedProduct.Id,
                    updatedProduct.CategoryId,
                    updatedProduct.BrandId,
                    updatedProduct.Name,
                    updatedProduct.ShortDescription,
                    updatedProduct.FullDescription,
                    updatedProduct.Slug,
                    updatedProduct.ThumbnailUrl,
                    updatedProduct.Ingredients,
                    updatedProduct.UsageInstructions,
                    updatedProduct.Contraindications,
                    updatedProduct.StorageInstructions,
                    updatedProduct.RegistrationNumber,
                    updatedProduct.IsPrescriptionDrug,
                    updatedProduct.IsActive,
                    updatedProduct.IsFeatured,
                    updatedProduct.CreatedDate,
                    updatedProduct.UpdatedDate,
                    BrandName = updatedProduct.Brand?.Name,
                    CategoryName = updatedProduct.Category?.Name,
                    Images = updatedProduct.Images.Select(i => new
                    {
                        i.Id,
                        i.ImageUrl,
                        i.AltText,
                        i.DisplayOrder
                    }),
                    ProductVariants = updatedProduct.ProductVariants.Select(v => new
                    {
                        v.Id,
                        v.SKU,
                        v.Barcode,
                        v.Price,
                        v.OriginalPrice,
                        v.StockQuantity,
                        v.MaxSalesQuantity,
                        v.Weight,
                        v.Dimensions,
                        v.ImageUrl,
                        v.IsActive,
                        OptionValues = v.VariantOptionValues.Select(vov => new
                        {
                            OptionValueId = vov.ProductOptionValueId,
                            OptionValue = vov.ProductOptionValue.Value,
                            OptionId = vov.ProductOptionValue.ProductOption.Id,
                            OptionName = vov.ProductOptionValue.ProductOption.Name
                        })
                    })
                };

                return Ok(response);
            }, "Update Product");
        }

        /// <summary>
        /// Create a new ProductStatusMap
        /// </summary>
        /// <param name="dto">ProductStatusMap creation data</param>
        /// <returns>Created ProductStatusMap</returns>
        [HttpPost("status-map")]
        public async Task<ActionResult<ProductStatusMapResponseDto>> CreateProductStatusMap([FromBody] CreateProductStatusMapDto dto)
        {
            var result = await _productService.CreateProductStatusMapAsync(dto);
            return Ok(result);     
        }

        [HttpPost("cart")]
        public async Task<ActionResult<dynamic>> CartInfo(IList<CartItem> cartItems)
        {
            var cartProductDtos = await _cartService.GetCartProductsAsync(cartItems);

            return Ok(cartProductDtos);
        }
    }
}
