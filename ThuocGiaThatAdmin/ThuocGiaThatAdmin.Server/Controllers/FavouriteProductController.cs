using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ThuocGiaThatAdmin.Service.Services;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    public class FavouriteProductController : BaseApiController
    {
        private readonly FavouriteProductService _service;

        public FavouriteProductController(FavouriteProductService service, ILogger<FavouriteProductController> logger) : base(logger)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] FavouriteRequest request)
        {
            await _service.AddFavoriteAsync(request.CustomerId, request.ProductVariantId, request.Type);
            return Ok(new { message = "Added to favorites" });
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFavorite([FromQuery] int customerId, [FromQuery] int productVariantId, [FromQuery] FavouriteProductType type)
        {
            await _service.RemoveFavoriteAsync(customerId, productVariantId, type);
            return Ok(new { message = "Removed from favorites" });
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites([FromQuery] int customerId, [FromQuery] FavouriteProductType? type, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetFavoritesAsync(customerId, type, page, pageSize);
            return Ok(result);
        }
    }

    public class FavouriteRequest
    {
        public int CustomerId { get; set; }
        public int ProductVariantId { get; set; }
        public FavouriteProductType Type { get; set; }
    }
}
