using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Service.Services;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    [ApiController]
    [Route("api/product-collection")]
    public class ProductCollectionController : BaseApiController
    {
        private readonly IProductCollectionService _service;

        public ProductCollectionController(IProductCollectionService service, ILogger<ProductCollectionController> logger) 
            : base(logger)
        {
            _service = service;
        }

        [HttpGet("high-profit")]
        public async Task<IActionResult> GetHighProfit([FromQuery] decimal minRevenue = 100000000)
        {
            return await ExecuteActionAsync(async () =>
            {
                var products = await _service.GetHighProfitProductsAsync(minRevenue);
                return Ok(products);
            }, "Get High Profit Products");
        }

        [HttpGet("top-selling")]
        public async Task<IActionResult> GetTopSelling([FromQuery] int minQuantity = 100)
        {
            return await ExecuteActionAsync(async () =>
            {
                var products = await _service.GetTopSellingProductsAsync(minQuantity);
                return Ok(products);
            }, "Get Top Selling Products");
        }

        [HttpGet("new")]
        public async Task<IActionResult> GetNew([FromQuery] int days = 30)
        {
            return await ExecuteActionAsync(async () =>
            {
                var products = await _service.GetNewProductsAsync(days);
                return Ok(products);
            }, "Get New Products");
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock([FromQuery] int maxStock = 100)
        {
            return await ExecuteActionAsync(async () =>
            {
                var products = await _service.GetLowStockProductsAsync(maxStock);
                return Ok(products);
            }, "Get Low Stock Products");
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            return await ExecuteActionAsync(async () =>
            {
                var products = await _service.GetCollectionProductsAsync(slug);
                return Ok(products);
            }, $"Get Collection {slug}");
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts([FromQuery] string collection)
        {
            return await ExecuteActionAsync(async () =>
            {
                var products = await _service.GetCollectionProductsAsync(collection);
                return Ok(products);
            }, $"Get Products for Collection {collection}");
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCollectionDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var collection = await _service.CreateCollectionAsync(dto);
                return CreatedAtAction(nameof(GetBySlug), new { slug = collection.Slug }, collection);
            }, "Create Collection");
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductCollectionDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var collection = await _service.UpdateCollectionAsync(id, dto);
                return Ok(collection);
            }, "Update Collection");
        }

        [HttpPost("{id}/products")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProducts(int id, [FromBody] List<int> productIds)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _service.AddProductsToCollectionAsync(id, productIds);
                return Ok(new { message = "Products added successfully" });
            }, "Add Products to Collection");
        }
        
        [HttpGet("max-order-config")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMaxOrderConfigs()
        {
            return await ExecuteActionAsync(async () =>
            {
                var products = await _service.GetProductsWithMaxOrderAsync();
                return Ok(products);
            }, "Get Max Order Configs");
        }
        
        [HttpPost("max-order-config/{productId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetMaxOrderConfig(int productId, [FromBody] SetMaxOrderDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _service.SetMaxOrderConfigAsync(productId, dto);
                return Ok(new { message = "Max order config updated successfully" });
            }, "Set Max Order Config");
        }
        
    }
}
