using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;
using ThuocGiaThatAdmin.Contracts.DTOs;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : BaseApiController
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService, ILogger<BrandController> logger) : base(logger)
        {
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
        }

        /// <summary>
        /// Get brands with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <returns>Paginated list of brands</returns>
        [HttpGet]
        public async Task<IActionResult> GetBrands([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (brands, totalCount) = await _brandService.GetBrandsAsync(pageNumber, pageSize);

                var brandDtos = brands.Select(brand => new BrandResponseDto
                {
                    Id = brand.Id,
                    Name = brand.Name,
                    Slug = brand.Slug,
                    CountryOfOrigin = brand.CountryOfOrigin,
                    Website = brand.Website,
                    LogoUrl = brand.LogoUrl,
                    IsActive = brand.IsActive,
                    CreatedDate = brand.CreatedDate,
                    UpdatedDate = brand.UpdatedDate
                });

                return SuccessPaginated(brandDtos, pageNumber, pageSize, totalCount);
            }, "Get Brands");
        }

        /// <summary>
        /// Get brand by ID
        /// </summary>
        /// <param name="id">Brand ID</param>
        /// <returns>Brand details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var brand = await _brandService.GetBrandByIdAsync(id);

                if (brand == null)
                {
                    return NotFoundResponse($"Brand with ID {id} not found");
                }

                var brandDto = new BrandResponseDto
                {
                    Id = brand.Id,
                    Name = brand.Name,
                    Slug = brand.Slug,
                    CountryOfOrigin = brand.CountryOfOrigin,
                    Website = brand.Website,
                    LogoUrl = brand.LogoUrl,
                    IsActive = brand.IsActive,
                    CreatedDate = brand.CreatedDate,
                    UpdatedDate = brand.UpdatedDate
                };

                return Success(brandDto);
            }, "Get Brand By Id");
        }

        /// <summary>
        /// Create a new brand
        /// </summary>
        /// <param name="dto">Brand creation data</param>
        /// <returns>Created brand</returns>
        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromBody] CreateBrandDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var brand = new Brand
                {
                    Name = dto.Name,
                    Slug = dto.Slug,
                    CountryOfOrigin = dto.CountryOfOrigin,
                    Website = dto.Website,
                    LogoUrl = dto.LogoUrl,
                    IsActive = dto.IsActive
                };

                await _brandService.CreateAsync(brand);

                var brandDto = new BrandResponseDto
                {
                    Id = brand.Id,
                    Name = brand.Name,
                    Slug = brand.Slug,
                    CountryOfOrigin = brand.CountryOfOrigin,
                    Website = brand.Website,
                    LogoUrl = brand.LogoUrl,
                    IsActive = brand.IsActive,
                    CreatedDate = brand.CreatedDate,
                    UpdatedDate = brand.UpdatedDate
                };

                return Created(brandDto, "Brand created successfully");
            }, "Create Brand");
        }

        /// <summary>
        /// Update an existing brand
        /// </summary>
        /// <param name="id">Brand ID</param>
        /// <param name="dto">Brand update data</param>
        /// <returns>Updated brand confirmation</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrand(int id, [FromBody] UpdateBrandDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var brand = new Brand
                {
                    Id = id,
                    Name = dto.Name,
                    Slug = dto.Slug,
                    CountryOfOrigin = dto.CountryOfOrigin,
                    Website = dto.Website,
                    LogoUrl = dto.LogoUrl,
                    IsActive = dto.IsActive
                };

                var result = await _brandService.UpdateBrandAsync(brand);

                if (result > 0)
                {
                    // Fetch updated brand to return
                    var updatedBrand = await _brandService.GetBrandByIdAsync(id);
                    var brandDto = new BrandResponseDto
                    {
                        Id = updatedBrand!.Id,
                        Name = updatedBrand.Name,
                        Slug = updatedBrand.Slug,
                        CountryOfOrigin = updatedBrand.CountryOfOrigin,
                        Website = updatedBrand.Website,
                        LogoUrl = updatedBrand.LogoUrl,
                        IsActive = updatedBrand.IsActive,
                        CreatedDate = updatedBrand.CreatedDate,
                        UpdatedDate = updatedBrand.UpdatedDate
                    };
                    return Success(brandDto, "Brand updated successfully");
                }

                return InternalServerErrorResponse("Failed to update brand");
            }, "Update Brand");
        }

        /// <summary>
        /// Delete a brand by ID
        /// </summary>
        /// <param name="id">Brand ID</param>
        /// <returns>Deletion confirmation</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var result = await _brandService.DeleteBrandAsync(id);

                if (result > 0)
                    return Success(new { brandId = id }, "Brand deleted successfully");

                return InternalServerErrorResponse("Failed to delete brand");
            }, "Delete Brand");
        }
    }
}
