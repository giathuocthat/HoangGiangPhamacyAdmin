using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesRegionController : BaseApiController
    {
        private readonly ISalesRegionService _salesRegionService;

        public SalesRegionController(
            ISalesRegionService salesRegionService,
            ILogger<SalesRegionController> logger) : base(logger)
        {
            _salesRegionService = salesRegionService;
        }

        /// <summary>
        /// GET: api/salesregion
        /// Lấy danh sách tất cả regions đang active
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllRegions()
        {
            return await ExecuteActionAsync(async () =>
            {
                var regions = await _salesRegionService.GetAllRegionsAsync();
                return Success(regions);
            });
        }

        /// <summary>
        /// GET: api/salesregion/{id}
        /// Lấy thông tin chi tiết region theo ID với thống kê
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRegionById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var region = await _salesRegionService.GetRegionByIdAsync(id);

                if (region == null)
                {
                    return NotFoundResponse("Region not found");
                }

                return Success(region);
            });
        }

        /// <summary>
        /// POST: api/salesregion
        /// Tạo region mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateRegion([FromBody] CreateSalesRegionDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (success, message, region) = await _salesRegionService.CreateRegionAsync(dto);

                if (!success)
                {
                    return BadRequestResponse(message);
                }

                return Created($"/api/salesregion/{region!.Id}", region);
            });
        }

        /// <summary>
        /// PUT: api/salesregion/{id}
        /// Cập nhật thông tin region
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRegion(int id, [FromBody] UpdateSalesRegionDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (success, message, region) = await _salesRegionService.UpdateRegionAsync(id, dto);

                if (!success)
                {
                    if (message == "Region not found")
                    {
                        return NotFoundResponse(message);
                    }
                    return BadRequestResponse(message);
                }

                return Success(region, message);
            });
        }

        /// <summary>
        /// DELETE: api/salesregion/{id}
        /// Xóa region (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegion(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (success, message) = await _salesRegionService.DeleteRegionAsync(id);

                if (!success)
                {
                    if (message == "Region not found")
                    {
                        return NotFoundResponse(message);
                    }
                    return BadRequestResponse(message);
                }

                return Success(new { message });
            });
        }
    }
}
