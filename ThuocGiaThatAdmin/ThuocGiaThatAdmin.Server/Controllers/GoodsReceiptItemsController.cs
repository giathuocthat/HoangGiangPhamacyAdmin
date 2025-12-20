using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Enums;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    /// <summary>
    /// API Controller for Goods Receipt Item management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GoodsReceiptItemsController : BaseApiController
    {
        private readonly IGoodsReceiptItemService _itemService;

        public GoodsReceiptItemsController(
            IGoodsReceiptItemService itemService,
            ILogger<GoodsReceiptItemsController> logger) : base(logger)
        {
            _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
        }

        /// <summary>
        /// Get all goods receipt items
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteActionAsync(async () =>
            {
                var items = await _itemService.GetAllAsync();
                return Success(items);
            }, "Get All Goods Receipt Items");
        }

        /// <summary>
        /// Get goods receipt item by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var item = await _itemService.GetByIdAsync(id);
                if (item == null)
                    return NotFoundResponse($"Goods receipt item with ID {id} not found");

                return Success(item);
            }, "Get Goods Receipt Item By Id");
        }

        /// <summary>
        /// Get goods receipt item with full details
        /// </summary>
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetWithDetails(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var item = await _itemService.GetWithDetailsAsync(id);
                if (item == null)
                    return NotFoundResponse($"Goods receipt item with ID {id} not found");

                return Success(item);
            }, "Get Goods Receipt Item Details");
        }

        /// <summary>
        /// Get goods receipt items by goods receipt ID
        /// </summary>
        [HttpGet("goods-receipt/{goodsReceiptId}")]
        public async Task<IActionResult> GetByGoodsReceiptId(int goodsReceiptId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var items = await _itemService.GetByGoodsReceiptIdAsync(goodsReceiptId);
                return Success(items);
            }, "Get Goods Receipt Items By Goods Receipt");
        }

        /// <summary>
        /// Get goods receipt items by purchase order item ID
        /// </summary>
        [HttpGet("purchase-order-item/{purchaseOrderItemId}")]
        public async Task<IActionResult> GetByPurchaseOrderItemId(int purchaseOrderItemId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var items = await _itemService.GetByPurchaseOrderItemIdAsync(purchaseOrderItemId);
                return Success(items);
            }, "Get Goods Receipt Items By Purchase Order Item");
        }

        /// <summary>
        /// Get goods receipt items by quality status
        /// </summary>
        [HttpGet("goods-receipt/{goodsReceiptId}/quality-status/{qualityStatus}")]
        public async Task<IActionResult> GetByQualityStatus(int goodsReceiptId, QualityStatus qualityStatus)
        {
            return await ExecuteActionAsync(async () =>
            {
                var items = await _itemService.GetItemsByQualityStatusAsync(goodsReceiptId, qualityStatus);
                return Success(items);
            }, "Get Goods Receipt Items By Quality Status");
        }

        /// <summary>
        /// Get summary statistics for goods receipt items
        /// </summary>
        [HttpGet("goods-receipt/{goodsReceiptId}/summary")]
        public async Task<IActionResult> GetSummary(int goodsReceiptId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var summary = await _itemService.GetSummaryByGoodsReceiptIdAsync(goodsReceiptId);
                return Success(summary);
            }, "Get Goods Receipt Items Summary");
        }

        /// <summary>
        /// Get items expiring soon
        /// </summary>
        [HttpGet("expiring-soon")]
        public async Task<IActionResult> GetExpiringSoon([FromQuery] int daysThreshold = 30)
        {
            return await ExecuteActionAsync(async () =>
            {
                var items = await _itemService.GetExpiringSoonItemsAsync(daysThreshold);
                return Success(items);
            }, "Get Expiring Soon Items");
        }

        /// <summary>
        /// Get expired items
        /// </summary>
        [HttpGet("expired")]
        public async Task<IActionResult> GetExpired()
        {
            return await ExecuteActionAsync(async () =>
            {
                var items = await _itemService.GetExpiredItemsAsync();
                return Success(items);
            }, "Get Expired Items");
        }

        /// <summary>
        /// Create a new goods receipt item
        /// </summary>
        [HttpPost("goods-receipt/{goodsReceiptId}")]
        public async Task<IActionResult> Create(int goodsReceiptId, [FromBody] CreateGoodsReceiptItemDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var item = await _itemService.CreateAsync(goodsReceiptId, dto);
                return Success(item, "Goods receipt item created successfully");
            }, "Create Goods Receipt Item");
        }

        /// <summary>
        /// Update an existing goods receipt item
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGoodsReceiptItemDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var item = await _itemService.UpdateAsync(id, dto);
                return Success(item, "Goods receipt item updated successfully");
            }, "Update Goods Receipt Item");
        }

        /// <summary>
        /// Update quality inspection for an item
        /// </summary>
        [HttpPut("{id}/quality-inspection")]
        public async Task<IActionResult> UpdateQualityInspection(int id, [FromBody] UpdateQualityInspectionDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var item = await _itemService.UpdateQualityInspectionAsync(id, dto);
                return Success(item, "Quality inspection updated successfully");
            }, "Update Quality Inspection");
        }

        /// <summary>
        /// Update warehouse location for an item
        /// </summary>
        [HttpPut("{id}/location")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] UpdateItemLocationDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var item = await _itemService.UpdateLocationAsync(id, dto);
                return Success(item, "Item location updated successfully");
            }, "Update Item Location");
        }

        /// <summary>
        /// Delete a goods receipt item
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _itemService.DeleteAsync(id);
                return Success(new { id }, "Goods receipt item deleted successfully");
            }, "Delete Goods Receipt Item");
        }
    }
}
