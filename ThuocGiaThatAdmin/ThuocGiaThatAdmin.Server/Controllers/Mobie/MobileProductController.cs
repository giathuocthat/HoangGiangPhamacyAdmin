using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThuocGiaThatAdmin.Service.Interfaces;
using ThuocGiaThatAdmin.Service.Services;

namespace ThuocGiaThatAdmin.Server.Controllers.Mobie
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileProductController : BaseApiController
    {
        private readonly ProductService _productService;
        private readonly ICartService _cartService;

        public MobileProductController(ProductService productService, ILogger<MobileProductController> logger, ICartService cartService) : base(logger)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
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
                    product.ThumbnailUrl,
                    product.IsActive,
                    product.IsFeatured,
                    product.CreatedDate,
                    product.UpdatedDate,
                    Brand = new
                    {
                        product.Brand?.Id,
                        product.Brand?.Name,
                        product.Brand?.Slug,
                        product.Brand?.CountryOfOrigin,
                        product.Brand?.LogoUrl
                    },
                    Category = new
                    {
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
    }
}
