using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThuocGiaThat.Infrastucture;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVariantController : ControllerBase
    {
        private readonly IRepository<ProductVariant> _productVariantRepository;
        private readonly TrueMecContext _context; 

        public ProductVariantController(IRepository<ProductVariant> productVariantRepository, TrueMecContext context)
        {
            _productVariantRepository = productVariantRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetAllProductVariant()
        {
            var productVariants = await _context.ProductVariants.ToListAsync();
            var products = await _context.Products.Select(x => new
            {
                x.Id,
                x.Name,
                x.IsActive
            }).ToListAsync();

            var innerJoinResult = productVariants.Join(products, variant => variant.Id, product => product.Id, (variant, product) => new
            {
                variant.Id,
                variant.ProductId,
                variant.SKU,
                variant.Barcode,
                variant.Price,
                variant.OriginalPrice,
                variant.StockQuantity,
                variant.MaxSalesQuantity,
                variant.Weight,
                variant.Dimensions,
                variant.ImageUrl,
                variant.IsActive,
                variant.CreatedDate,
                variant.UpdatedDate,
                productName = product.Name

            });

            return Ok(innerJoinResult);
        }
    }
}
