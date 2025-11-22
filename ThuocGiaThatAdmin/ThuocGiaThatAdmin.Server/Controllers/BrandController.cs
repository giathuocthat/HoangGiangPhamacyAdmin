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
    public class BrandController : ControllerBase
    {
        private readonly BrandService _brandService;
        private readonly ILogger<BrandController> _logger;

        public BrandController(BrandService brandService, ILogger<BrandController> logger)
        {
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _logger = logger;
        }

        /// <summary>
        /// Get brands with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <returns>Paginated list of brands</returns>
        [HttpGet]
        public async Task<ActionResult<dynamic>> GetBrands([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (brands, totalCount) = await _brandService.GetBrandsAsync(pageNumber, pageSize);

                var response = new
                {
                    Data = brands.Select(brand => new
                    {
                        brand.Id,
                        brand.Name,
                        brand.Slug,
                        brand.CountryOfOrigin,
                        brand.Website,
                        brand.LogoUrl,
                        brand.IsActive,
                        brand.CreatedDate,
                        brand.UpdatedDate
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
                _logger.LogError(ex, "Error getting brands");
                return StatusCode(500, new { message = "An error occurred while retrieving brands", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing brand
        /// </summary>
        /// <param name="id">Brand ID</param>
        /// <param name="brand">Brand data to update</param>
        /// <returns>Updated brand confirmation</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBrand(int id, [FromBody] Brand brand)
        {
            try
            {
                if (id != brand.Id)
                    return BadRequest(new { message = "ID in URL does not match ID in request body" });

                var result = await _brandService.UpdateBrandAsync(brand);

                if (result > 0)
                    return Ok(new { message = "Brand updated successfully", brandId = id });

                return StatusCode(500, new { message = "Failed to update brand" });
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
                _logger.LogError(ex, "Error updating brand");
                return StatusCode(500, new { message = "An error occurred while updating the brand", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a brand by ID
        /// </summary>
        /// <param name="id">Brand ID</param>
        /// <returns>Deletion confirmation</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBrand(int id)
        {
            try
            {
                var result = await _brandService.DeleteBrandAsync(id);

                if (result > 0)
                    return Ok(new { message = "Brand deleted successfully", brandId = id });

                return StatusCode(500, new { message = "Failed to delete brand" });
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
                _logger.LogError(ex, "Error deleting brand");
                return StatusCode(500, new { message = "An error occurred while deleting the brand", error = ex.Message });
            }
        }
    }
}
