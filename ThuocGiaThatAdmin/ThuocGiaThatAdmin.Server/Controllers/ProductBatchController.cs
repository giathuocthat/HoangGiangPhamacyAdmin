using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ThuocGiaThatAdmin.Contracts.DTOs.ProductBatch;
using ThuocGiaThatAdmin.Service.Services;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    public class ProductBatchController : BaseApiController
    {
        private readonly ProductBatchService _productBatchService;

        public ProductBatchController(
            ProductBatchService productBatchService,
            ILogger<ProductBatchController> logger) : base(logger)
        {
            _productBatchService = productBatchService;
        }

        [HttpGet("lookup/{batchNumber}")]
        public async Task<IActionResult> GetBatchByNumber(string batchNumber)
        {
            return await ExecuteActionAsync(async () =>
            {
                var batch = await _productBatchService.GetBatchByNumberAsync(batchNumber);
                if (batch == null)
                {
                    return NotFound(new { message = $"Batch number '{batchNumber}' not found." });
                }
                return Ok(batch);
            }, "Get Batch By Number");
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBatch([FromBody] CreateBatchDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                // In a real app, get generic user id from User.Identity
                // For now, hardcode or leave as 0 if user context not fully set up in this context
                int userId = 0; 
                // Try to parse from claim if available, otherwise 0
                if (User.Identity.IsAuthenticated)
                {
                     // Implementation depends on how BaseApiController or Claim parsing is set up
                     // Assuming basic handling for now
                }

                var createdBatch = await _productBatchService.CreateBatchAsync(dto, userId);
                return Created(createdBatch, "Batch created successfully");
            }, "Create Batch");
        }
    }
}
