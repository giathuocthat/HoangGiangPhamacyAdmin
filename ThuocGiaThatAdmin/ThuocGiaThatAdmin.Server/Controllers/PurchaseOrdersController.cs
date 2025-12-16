using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Enums;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    /// <summary>
    /// API Controller for Purchase Order management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseOrdersController : BaseApiController
    {
        private readonly IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrdersController(
            IPurchaseOrderService purchaseOrderService,
            ILogger<PurchaseOrdersController> logger) : base(logger)
        {
            _purchaseOrderService = purchaseOrderService ?? throw new ArgumentNullException(nameof(purchaseOrderService));
        }

        /// <summary>
        /// Get all purchase orders
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteActionAsync(async () =>
            {
                var orders = await _purchaseOrderService.GetAllAsync();
                return Success(orders);
            }, "Get All Purchase Orders");
        }

        /// <summary>
        /// Get purchase order by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var order = await _purchaseOrderService.GetByIdAsync(id);
                if (order == null)
                    return NotFoundResponse($"Purchase order with ID {id} not found");

                return Success(order);
            }, "Get Purchase Order By Id");
        }

        /// <summary>
        /// Get purchase order by order number
        /// </summary>
        [HttpGet("order-number/{orderNumber}")]
        public async Task<IActionResult> GetByOrderNumber(string orderNumber)
        {
            return await ExecuteActionAsync(async () =>
            {
                var order = await _purchaseOrderService.GetByOrderNumberAsync(orderNumber);
                if (order == null)
                    return NotFoundResponse($"Purchase order with number '{orderNumber}' not found");

                return Success(order);
            }, "Get Purchase Order By Number");
        }

        /// <summary>
        /// Get purchase order with full details
        /// </summary>
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetWithDetails(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var order = await _purchaseOrderService.GetWithDetailsAsync(id);
                if (order == null)
                    return NotFoundResponse($"Purchase order with ID {id} not found");

                return Success(order);
            }, "Get Purchase Order Details");
        }

        /// <summary>
        /// Get purchase orders by supplier
        /// </summary>
        [HttpGet("supplier/{supplierId}")]
        public async Task<IActionResult> GetBySupplier(int supplierId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var orders = await _purchaseOrderService.GetBySupplierIdAsync(supplierId);
                return Success(orders);
            }, "Get Purchase Orders By Supplier");
        }

        /// <summary>
        /// Get purchase orders by warehouse
        /// </summary>
        [HttpGet("warehouse/{warehouseId}")]
        public async Task<IActionResult> GetByWarehouse(int warehouseId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var orders = await _purchaseOrderService.GetByWarehouseIdAsync(warehouseId);
                return Success(orders);
            }, "Get Purchase Orders By Warehouse");
        }

        /// <summary>
        /// Get purchase orders by status
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(PurchaseOrderStatus status)
        {
            return await ExecuteActionAsync(async () =>
            {
                var orders = await _purchaseOrderService.GetByStatusAsync(status);
                return Success(orders);
            }, "Get Purchase Orders By Status");
        }

        /// <summary>
        /// Get paged purchase orders
        /// </summary>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? supplierId = null,
            [FromQuery] int? warehouseId = null,
            [FromQuery] PurchaseOrderStatus? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (orders, totalCount) = await _purchaseOrderService.GetPagedPurchaseOrdersAsync(
                    pageNumber, pageSize, searchTerm, supplierId, warehouseId, status, fromDate, toDate);

                var response = new
                {
                    Data = orders,
                    Pagination = new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                    }
                };

                return Success(response);
            }, "Get Paged Purchase Orders");
        }

        /// <summary>
        /// Generate next order number
        /// </summary>
        [HttpGet("generate-order-number")]
        public async Task<IActionResult> GenerateOrderNumber()
        {
            return await ExecuteActionAsync(async () =>
            {
                var orderNumber = await _purchaseOrderService.GenerateOrderNumberAsync();
                return Success(new { orderNumber });
            }, "Generate Order Number");
        }

        /// <summary>
        /// Create a new purchase order
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var order = await _purchaseOrderService.CreateAsync(dto, userId);
                return Success(order, "Purchase order created successfully");
            }, "Create Purchase Order");
        }

        /// <summary>
        /// Update an existing purchase order
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePurchaseOrderDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var order = await _purchaseOrderService.UpdateAsync(id, dto);
                return Success(order, "Purchase order updated successfully");
            }, "Update Purchase Order");
        }

        /// <summary>
        /// Approve a purchase order
        /// </summary>
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id, [FromBody] ApprovePurchaseOrderDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var order = await _purchaseOrderService.ApproveAsync(id, userId, dto);
                return Success(order, "Purchase order approved successfully");
            }, "Approve Purchase Order");
        }

        /// <summary>
        /// Cancel a purchase order
        /// </summary>
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id, [FromBody] CancelPurchaseOrderDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var order = await _purchaseOrderService.CancelAsync(id, userId, dto);
                return Success(order, "Purchase order cancelled successfully");
            }, "Cancel Purchase Order");
        }

        /// <summary>
        /// Delete a purchase order
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _purchaseOrderService.DeleteAsync(id);
                return Success(new { id }, "Purchase order deleted successfully");
            }, "Delete Purchase Order");
        }
    }
}
