using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Services;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductOptionController : ControllerBase
    {
        private readonly ProductOptionService _productOptionService;
        private readonly ILogger<ProductOptionController> _logger;

        public ProductOptionController(ProductOptionService productOptionService, ILogger<ProductOptionController> logger)
        {
            _productOptionService = productOptionService ?? throw new ArgumentNullException(nameof(productOptionService));
            _logger = logger;
        }

        /// <summary>
        /// Get product options with pagination, including their values
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <returns>Paginated list of product options with their values</returns>
        [HttpGet]
        public async Task<ActionResult<dynamic>> GetProductOptions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (productOptions, totalCount) = await _productOptionService.GetProductOptionsAsync(pageNumber, pageSize);

                var response = new
                {
                    Data = productOptions.Select(po => new
                    {
                        po.Id,
                        po.ProductId,
                        po.Name,
                        po.DisplayOrder,
                        ProductOptionValues = po.ProductOptionValues.Select(v => new
                        {
                            v.Id,
                            v.ProductOptionId,
                            v.Value,
                            v.DisplayOrder
                        }).OrderBy(v => v.DisplayOrder)
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
                _logger.LogError(ex, "Error getting product options");
                return StatusCode(500, new { message = "An error occurred while retrieving product options", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing product option with its values
        /// </summary>
        /// <param name="id">ProductOption ID</param>
        /// <param name="productOption">ProductOption data to update (including values)</param>
        /// <returns>Updated product option confirmation</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProductOption(int id, [FromBody] ProductOption productOption)
        {
            try
            {
                if (id != productOption.Id)
                    return BadRequest(new { message = "ID in URL does not match ID in request body" });

                var result = await _productOptionService.UpdateProductOptionAsync(productOption);

                if (result > 0)
                    return Ok(new { message = "ProductOption updated successfully", productOptionId = id });

                return StatusCode(500, new { message = "Failed to update product option" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product option");
                return StatusCode(500, new { message = "An error occurred while updating the product option", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a product option by ID
        /// </summary>
        /// <param name="id">ProductOption ID</param>
        /// <returns>Deletion confirmation</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProductOption(int id)
        {
            try
            {
                var result = await _productOptionService.DeleteProductOptionAsync(id);

                if (result > 0)
                    return Ok(new { message = "ProductOption deleted successfully", productOptionId = id });

                return StatusCode(500, new { message = "Failed to delete product option" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product option");
                return StatusCode(500, new { message = "An error occurred while deleting the product option", error = ex.Message });
            }
        }
    }
}
