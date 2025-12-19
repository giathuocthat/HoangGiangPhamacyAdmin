using Microsoft.AspNetCore.Mvc;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Service.Services;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    public class WarehouseLocationController : BaseApiController
    {
        private readonly WarehouseLocationService _locationService;

        public WarehouseLocationController(
            WarehouseLocationService locationService,
            ILogger<WarehouseLocationController> logger) : base(logger)
        {
            _locationService = locationService;
        }

        /// <summary>
        /// Get all warehouse locations with pagination and filtering
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllLocations(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchText = null,
            [FromQuery] int? warehouseId = null,
            [FromQuery] bool? isActive = null)
        {
            return await ExecuteActionAsync(async () =>
            {
                if (pageSize > 100)
                {
                    return BadRequestResponse("Page size cannot exceed 100");
                }

                var result = await _locationService.GetAllLocationsAsync(
                    pageNumber, pageSize, searchText, warehouseId, isActive);

                var response = new
                {
                    Data = result.Items,
                    Pagination = new
                    {
                        PageNumber = result.PageNumber,
                        PageSize = result.PageSize,
                        TotalCount = result.TotalCount,
                        TotalPages = (int)Math.Ceiling(result.TotalCount / (double)result.PageSize)
                    }
                };

                return Success(response, "Warehouse locations retrieved successfully");
            }, "Get All Warehouse Locations");
        }

        /// <summary>
        /// Get warehouse location by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocationById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var location = await _locationService.GetLocationByIdAsync(id);
                if (location == null)
                {
                    return NotFoundResponse($"Location with ID {id} not found");
                }

                return Success(location, "Location retrieved successfully");
            }, "Get Location By ID");
        }

        /// <summary>
        /// Get warehouse location by code
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetLocationByCode(string code)
        {
            return await ExecuteActionAsync(async () =>
            {
                var location = await _locationService.GetLocationByCodeAsync(code);
                if (location == null)
                {
                    return NotFoundResponse($"Location with code '{code}' not found");
                }

                return Success(location, "Location retrieved successfully");
            }, "Get Location By Code");
        }

        /// <summary>
        /// Get all locations for a specific warehouse
        /// </summary>
        [HttpGet("warehouse/{warehouseId}")]
        public async Task<IActionResult> GetLocationsByWarehouse(int warehouseId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var locations = await _locationService.GetLocationsByWarehouseAsync(warehouseId);
                return Success(locations, "Warehouse locations retrieved successfully");
            }, "Get Locations By Warehouse");
        }

        /// <summary>
        /// Create new warehouse location
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateLocation([FromBody] CreateWarehouseLocationDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var location = await _locationService.CreateLocationAsync(dto);
                return Created(location, "Location created successfully");
            }, "Create Warehouse Location");
        }

        /// <summary>
        /// Update warehouse location
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] UpdateWarehouseLocationDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var location = await _locationService.UpdateLocationAsync(id, dto);
                return Success(location, "Location updated successfully");
            }, "Update Warehouse Location");
        }

        /// <summary>
        /// Mark location as inactive
        /// </summary>
        [HttpPut("{id}/inactive")]
        public async Task<IActionResult> SetLocationInactive(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var location = await _locationService.SetLocationActiveStatusAsync(id, false);
                return Success(location, "Location marked as inactive");
            }, "Set Location Inactive");
        }

        /// <summary>
        /// Mark location as active
        /// </summary>
        [HttpPut("{id}/active")]
        public async Task<IActionResult> SetLocationActive(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var location = await _locationService.SetLocationActiveStatusAsync(id, true);
                return Success(location, "Location marked as active");
            }, "Set Location Active");
        }
    }
}
