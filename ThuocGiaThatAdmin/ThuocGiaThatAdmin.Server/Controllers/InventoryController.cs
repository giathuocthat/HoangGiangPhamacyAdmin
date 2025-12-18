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
        /// Get all batches of a specific inventory
        /// </summary>
        /// <param name="id">Inventory ID</param>
        /// <returns>List of batches with detailed information</returns>
        [HttpGet("{id}/batches")]
        public async Task<IActionResult> GetInventoryBatches(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var batches = await _inventoryService.GetInventoryBatchesAsync(id);
                return Success(batches, "Inventory batches retrieved successfully");
            }, "Get Inventory Batches");
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

        /// <summary>
        /// Get inventories with pagination and search
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <param name="searchText">Search by product name, SKU, or warehouse name</param>
        /// <returns>Paginated list of inventories</returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetInventories(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchText = null)
        {
            return await ExecuteActionAsync(async () =>
            {
                if (pageSize > 100)
                {
                    return BadRequestResponse("Page size cannot exceed 100");
                }

                var result = await _inventoryService.GetInventoriesAsync(pageNumber, pageSize, searchText);

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

                return Success(response, "Inventories retrieved successfully");
            }, "Get Inventories List");
        }

        /// <summary>
        /// Record sale transaction (xuất kho để giao hàng)
        /// </summary>
        [HttpPost("sale")]
        public async Task<IActionResult> SaleInventory([FromBody] SaleInventoryDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                // TODO: Get userId from authentication
                var result = await _inventoryService.SaleInventoryAsync(dto, userId: null);
                return Created(result, result.Message);
            }, "Sale Inventory");
        }

        /// <summary>
        /// Record customer return transaction (khách trả hàng)
        /// </summary>
        [HttpPost("return")]
        public async Task<IActionResult> ReturnInventory([FromBody] ReturnInventoryDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var result = await _inventoryService.ReturnInventoryAsync(dto, userId: null);
                return Created(result, result.Message);
            }, "Return Inventory");
        }

        /// <summary>
        /// Record return to supplier transaction (trả hàng cho nhà cung cấp)
        /// </summary>
        [HttpPost("return-to-supplier")]
        public async Task<IActionResult> ReturnToSupplier([FromBody] ReturnToSupplierDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var result = await _inventoryService.ReturnToSupplierAsync(dto, userId: null);
                return Created(result, result.Message);
            }, "Return To Supplier");
        }

        /// <summary>
        /// Record warehouse transfer (chuyển kho)
        /// Creates both TransferOut and TransferIn transactions
        /// </summary>
        [HttpPost("transfer")]
        public async Task<IActionResult> TransferInventory([FromBody] TransferInventoryDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var result = await _inventoryService.TransferInventoryAsync(dto, userId: null);
                return Created(result, result.Message);
            }, "Transfer Inventory");
        }

        /// <summary>
        /// Record inventory adjustment (kiểm kê/điều chỉnh)
        /// </summary>
        [HttpPost("adjustment")]
        public async Task<IActionResult> AdjustInventory([FromBody] AdjustmentInventoryDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var result = await _inventoryService.AdjustInventoryAsync(dto, userId: null);
                return Created(result, result.Message);
            }, "Adjust Inventory");
        }
    }
}
