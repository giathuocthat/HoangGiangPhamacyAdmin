using Microsoft.AspNetCore.Mvc;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Service.Services;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    public class InventoryController : BaseApiController
    {
        private readonly InventoryService _inventoryService;

        public InventoryController(
            InventoryService inventoryService,
            ILogger<InventoryController> logger) : base(logger)
        {
            _inventoryService = inventoryService;
        }

        /// <summary>
        /// Purchase/receive inventory (nhập lô hàng vào kho)
        /// </summary>
        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseInventory([FromBody] PurchaseInventoryDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                // TODO: Get userId from authentication
                var result = await _inventoryService.PurchaseInventoryAsync(dto, userId: null);
                return Created(result, result.Message);
            }, "Purchase Inventory");
        }

        /// <summary>
        /// Get inventory by warehouse
        /// </summary>
        [HttpGet("warehouse/{warehouseId}")]
        public async Task<IActionResult> GetByWarehouse(int warehouseId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var inventories = await _inventoryService.GetInventoryByWarehouseAsync(warehouseId);
                return Ok(inventories);
            }, "Get Inventory By Warehouse");
        }

        /// <summary>
        /// Get low stock inventories
        /// </summary>
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock()
        {
            return await ExecuteActionAsync(async () =>
            {
                var inventories = await _inventoryService.GetLowStockInventoriesAsync();
                return Ok(inventories);
            }, "Get Low Stock Inventories");
        }
    }
}
