using Microsoft.AspNetCore.Mvc;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Service.Services;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    public class WarehouseController : BaseApiController
    {
        private readonly WarehouseService _warehouseService;

        public WarehouseController(
            WarehouseService warehouseService,
            ILogger<WarehouseController> logger) : base(logger)
        {
            _warehouseService = warehouseService;
        }

        /// <summary>
        /// Get all warehouses
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteActionAsync(async () =>
            {
                var warehouses = await _warehouseService.GetAllWarehousesAsync();
                return Ok(warehouses);
            }, "Get All Warehouses");
        }

        /// <summary>
        /// Get warehouse by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
                if (warehouse == null)
                {
                    return NotFound($"Warehouse with ID {id} not found");
                }
                return Ok(warehouse);
            }, "Get Warehouse By ID");
        }

        /// <summary>
        /// Get active warehouses
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            return await ExecuteActionAsync(async () =>
            {
                var warehouses = await _warehouseService.GetActiveWarehousesAsync();
                return Ok(warehouses);
            }, "Get Active Warehouses");
        }

        /// <summary>
        /// Create new warehouse
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWarehouseDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var warehouse = await _warehouseService.CreateWarehouseAsync(dto);
                return Created(warehouse, $"Warehouse '{warehouse.Name}' created successfully");
            }, "Create Warehouse");
        }

        /// <summary>
        /// Update warehouse
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateWarehouseDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var warehouse = await _warehouseService.UpdateWarehouseAsync(id, dto);
                return Ok(warehouse);
            }, "Update Warehouse");
        }

        /// <summary>
        /// Delete warehouse (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _warehouseService.DeleteWarehouseAsync(id);
                return Ok("Warehouse deleted successfully");
            }, "Delete Warehouse");
        }
    }
}
