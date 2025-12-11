using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ThuocGiaThatAdmin.Contract.Requests;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Domain.Enums;
using ThuocGiaThatAdmin.Server.Models;
using ThuocGiaThatAdmin.Service.Interfaces;
using ThuocGiaThatAdmin.Service.Services;

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
                var response = await _productService.CreateProductFromDtoAsync(dto);
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
                    product.Slug,
                    product.ThumbnailUrl,
                    product.Ingredients,
                    product.StorageInstructions,
                    product.RegistrationNumber,
                    product.IsPrescriptionDrug,
                    product.IsActive,
                    product.IsFeatured,
                    product.CreatedDate,
                    product.UpdatedDate,
                    product.DrugEfficacy,
                    product.DosageInstructions,
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
                    product.Specification,
                    product.IsPrescriptionDrug,
                    product.DrugEfficacy,
                    product.DosageInstructions,

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
                        product.DosageInstructions,
                        product.DrugEfficacy,
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
                // Update product using service
                await _productService.UpdateProductAsync(id, dto);

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
                    updatedProduct.DrugEfficacy,
                    updatedProduct.DosageInstructions,
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

        [HttpGet("collection/{type}")]
        public async Task<ActionResult<dynamic>> GetProductCollectionByTypeAsync(ProductStatusType type)
        {
            var result = await _productService.GetProductCollectionByTypeAsync(type);
            return Ok(result);
        }

        /// <summary>
        /// Search products by name or SKU for AutoComplete selection
        /// </summary>
        /// <param name="keyword">Search keyword</param>
        /// <param name="limit">Max results (default 20)</param>
        /// <returns>List of products with basic variant info</returns>
        [HttpGet("search-for-selection")]
        public async Task<ActionResult<dynamic>> SearchProductsForSelection([FromQuery] string keyword, [FromQuery] int limit = 20)
        {
            try
            {
                var results = await _productService.SearchProductsForSelectionAsync(keyword, limit);
                return Ok(results);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error searching products for selection");
                return StatusCode(500, new { message = "An error occurred while searching products", error = ex.Message });
            }
        }

        /// <summary>
        /// Get product variants with filtering, searching, and pagination
        /// </summary>
        /// <param name="request">Request parameters for filtering and pagination</param>
        /// <returns>Paginated list of product variants with product name, SKU, price, and option values</returns>
        [HttpGet("variants")]
        public async Task<ActionResult<dynamic>> GetProductVariants([FromQuery] GetProductVariantsRequestDto request)
        {
            try
            {
                var (variants, totalCount) = await _productService.GetProductVariantsAsync(request);

                var response = new
                {
                    Data = variants,
                    Pagination = new
                    {
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalCount = totalCount,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
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
                Logger.LogError(ex, "Error getting product variants");
                return StatusCode(500, new { message = "An error occurred while retrieving product variants", error = ex.Message });
            }
        }
    }
}
