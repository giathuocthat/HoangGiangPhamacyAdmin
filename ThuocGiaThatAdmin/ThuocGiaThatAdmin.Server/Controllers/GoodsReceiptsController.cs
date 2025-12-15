using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Enums;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    /// <summary>
    /// API Controller for Goods Receipt management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GoodsReceiptsController : BaseApiController
    {
        private readonly IGoodsReceiptService _goodsReceiptService;

        public GoodsReceiptsController(
            IGoodsReceiptService goodsReceiptService,
            ILogger<GoodsReceiptsController> logger) : base(logger)
        {
            _goodsReceiptService = goodsReceiptService ?? throw new ArgumentNullException(nameof(goodsReceiptService));
        }

        /// <summary>
        /// Get all goods receipts
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteActionAsync(async () =>
            {
                var receipts = await _goodsReceiptService.GetAllAsync();
                return Success(receipts);
            }, "Get All Goods Receipts");
        }

        /// <summary>
        /// Get goods receipt by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var receipt = await _goodsReceiptService.GetByIdAsync(id);
                if (receipt == null)
                    return NotFoundResponse($"Goods receipt with ID {id} not found");

                return Success(receipt);
            }, "Get Goods Receipt By Id");
        }

        /// <summary>
        /// Get goods receipt by receipt number
        /// </summary>
        [HttpGet("receipt-number/{receiptNumber}")]
        public async Task<IActionResult> GetByReceiptNumber(string receiptNumber)
        {
            return await ExecuteActionAsync(async () =>
            {
                var receipt = await _goodsReceiptService.GetByReceiptNumberAsync(receiptNumber);
                if (receipt == null)
                    return NotFoundResponse($"Goods receipt with number '{receiptNumber}' not found");

                return Success(receipt);
            }, "Get Goods Receipt By Number");
        }

        /// <summary>
        /// Get goods receipt with full details
        /// </summary>
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetWithDetails(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var receipt = await _goodsReceiptService.GetWithDetailsAsync(id);
                if (receipt == null)
                    return NotFoundResponse($"Goods receipt with ID {id} not found");

                return Success(receipt);
            }, "Get Goods Receipt Details");
        }

        /// <summary>
        /// Get goods receipts by purchase order
        /// </summary>
        [HttpGet("purchase-order/{purchaseOrderId}")]
        public async Task<IActionResult> GetByPurchaseOrder(int purchaseOrderId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var receipts = await _goodsReceiptService.GetByPurchaseOrderIdAsync(purchaseOrderId);
                return Success(receipts);
            }, "Get Goods Receipts By Purchase Order");
        }

        /// <summary>
        /// Get goods receipts by warehouse
        /// </summary>
        [HttpGet("warehouse/{warehouseId}")]
        public async Task<IActionResult> GetByWarehouse(int warehouseId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var receipts = await _goodsReceiptService.GetByWarehouseIdAsync(warehouseId);
                return Success(receipts);
            }, "Get Goods Receipts By Warehouse");
        }

        /// <summary>
        /// Get goods receipts by status
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(GoodsReceiptStatus status)
        {
            return await ExecuteActionAsync(async () =>
            {
                var receipts = await _goodsReceiptService.GetByStatusAsync(status);
                return Success(receipts);
            }, "Get Goods Receipts By Status");
        }

        /// <summary>
        /// Get paged goods receipts
        /// </summary>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? purchaseOrderId = null,
            [FromQuery] int? warehouseId = null,
            [FromQuery] GoodsReceiptStatus? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (receipts, totalCount) = await _goodsReceiptService.GetPagedGoodsReceiptsAsync(
                    pageNumber, pageSize, searchTerm, purchaseOrderId, warehouseId, status, fromDate, toDate);

                var response = new
                {
                    Data = receipts,
                    Pagination = new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                    }
                };

                return Success(response);
            }, "Get Paged Goods Receipts");
        }

        /// <summary>
        /// Generate next receipt number
        /// </summary>
        [HttpGet("generate-receipt-number")]
        public async Task<IActionResult> GenerateReceiptNumber()
        {
            return await ExecuteActionAsync(async () =>
            {
                var receiptNumber = await _goodsReceiptService.GenerateReceiptNumberAsync();
                return Success(new { receiptNumber });
            }, "Generate Receipt Number");
        }

        /// <summary>
        /// Create a new goods receipt
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGoodsReceiptDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var receipt = await _goodsReceiptService.CreateAsync(dto);
                return Success(receipt, "Goods receipt created successfully");
            }, "Create Goods Receipt");
        }

        /// <summary>
        /// Update an existing goods receipt
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGoodsReceiptDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var receipt = await _goodsReceiptService.UpdateAsync(id, dto);
                return Success(receipt, "Goods receipt updated successfully");
            }, "Update Goods Receipt");
        }

        /// <summary>
        /// Receive goods (with quality inspection)
        /// </summary>
        [HttpPost("{id}/receive")]
        public async Task<IActionResult> ReceiveGoods(int id, [FromBody] ReceiveGoodsDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var receipt = await _goodsReceiptService.ReceiveGoodsAsync(id, dto);
                return Success(receipt, "Goods received successfully");
            }, "Receive Goods");
        }

        /// <summary>
        /// Complete a goods receipt
        /// </summary>
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> Complete(int id, [FromBody] CompleteGoodsReceiptDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var receipt = await _goodsReceiptService.CompleteAsync(id, dto);
                return Success(receipt, "Goods receipt completed successfully");
            }, "Complete Goods Receipt");
        }

        /// <summary>
        /// Reject a goods receipt
        /// </summary>
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectGoodsReceiptDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var receipt = await _goodsReceiptService.RejectAsync(id, dto);
                return Success(receipt, "Goods receipt rejected");
            }, "Reject Goods Receipt");
        }

        /// <summary>
        /// Delete a goods receipt
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _goodsReceiptService.DeleteAsync(id);
                return Success(new { id }, "Goods receipt deleted successfully");
            }, "Delete Goods Receipt");
        }
    }
}
