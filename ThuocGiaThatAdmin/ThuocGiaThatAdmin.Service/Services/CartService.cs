using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contract.Requests;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class CartService : ICartService
    {
        private readonly IProductVariantRepository _productVariantRepository;

        public CartService(IProductVariantRepository productVariantRepository)
        {
            _productVariantRepository = productVariantRepository ?? throw new ArgumentNullException(nameof(productVariantRepository));
        }

        public async Task<IList<CartProductDto>> GetCartProductsAsync(IList<CartItem> cartItems)
        {
            var productVariants = await _productVariantRepository.GetByIdsAsync(cartItems.Select(ci => ci.ProductVariantId).ToList());
            var cartProductDtos = new List<CartProductDto>();
            foreach (var item in cartItems)
            {
                var productVariant = productVariants.FirstOrDefault(pv => pv.Id == item.ProductVariantId); 
                if (productVariant == null)
                {
                    continue;
                }

                var dto = new CartProductDto
                {
                    ProductId = productVariant.ProductId,
                    Quantity = item.Quantity,
                    MaxOrderQuantity = productVariant.StockQuantity,
                    Price = productVariant.Price,
                    OriginalPrice = productVariant.OriginalPrice,
                    ProductName = productVariant.Product.Name,
                    ImageUrl = productVariant.ImageUrl,
                    ProductVariantId = productVariant.Id,
                    BrandId = productVariant.Product.BrandId,
                    BrandName = productVariant.Product.Brand.Name
                };
                cartProductDtos.Add(dto);
            }
            return cartProductDtos;
        }
    }
}
