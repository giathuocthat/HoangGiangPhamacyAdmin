using Microsoft.AspNetCore.Mvc;
using ThuocGiaThatAdmin.Service.Services;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _logger = logger;
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
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products");
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

                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product by ID");
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
                _logger.LogError(ex, "Error getting products by category");
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
                _logger.LogError(ex, "Error searching products");
                return StatusCode(500, new { message = "An error occurred while searching products", error = ex.Message });
            }
        }
    }
}
