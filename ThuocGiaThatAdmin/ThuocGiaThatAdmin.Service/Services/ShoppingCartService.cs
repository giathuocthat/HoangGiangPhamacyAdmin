using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IShoppingCartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly IRepository<ProductVariant> _variantRepository;

        public ShoppingCartService(
            IShoppingCartRepository cartRepository,
            IShoppingCartItemRepository cartItemRepository,
            IProductRepository productRepository,
            IRepository<ProductVariant> variantRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
            _variantRepository = variantRepository;
        }

        public async Task<ShoppingCartDto> LoadCartAsync(int? customerId, string? sessionId, MergeLocalCartDto? localCart = null)
        {
            // 1. Get or create cart
            var cart = await GetOrCreateCartAsync(customerId, sessionId);

            // 2. Merge local cart items if provided
            if (localCart?.LocalCartItems?.Any() == true)
            {
                await MergeLocalCartItemsAsync(cart, localCart.LocalCartItems);
            }

            // 3. Validate all cart items
            await ValidateCartItemsAsync(cart);

            // 4. Recalculate totals
            RecalculateCartTotals(cart);

            await _cartRepository.SaveChangesAsync();

            return MapToDto(cart);
        }

        public async Task<CartSummaryDto> GetCartSummaryAsync(int? customerId, string? sessionId)
        {
            var cart = await GetCartAsync(customerId, sessionId);
            
            if (cart == null)
            {
                return new CartSummaryDto { TotalItems = 0, TotalAmount = 0, CartId = 0 };
            }

            return new CartSummaryDto
            {
                CartId = cart.Id,
                TotalItems = cart.TotalItems,
                TotalAmount = cart.TotalAmount
            };
        }

        public async Task<ShoppingCartDto> AddToCartAsync(AddToCartDto dto, int? customerId)
        {
            // Get or create cart
            var cart = await GetOrCreateCartAsync(customerId, dto.SessionId);

            // Get variant with product
            var variant = await _variantRepository.GetByIdAsync(dto.ProductVariantId);
            if (variant == null || !variant.IsActive)
                throw new InvalidOperationException("Product variant not found or inactive");

            var product = await _productRepository.GetByIdAsync(variant.ProductId);
            if (product == null || !product.IsActive)
                throw new InvalidOperationException("Product not found or inactive");

            // Check if item already exists in cart
            var existingItem = cart.CartItems.FirstOrDefault(i => i.ProductVariantId == dto.ProductVariantId);

            if (existingItem != null)
            {
                // Update quantity
                existingItem.Quantity += dto.Quantity;
                existingItem.TotalLineAmount = existingItem.Quantity * existingItem.UnitPrice;
                existingItem.UpdatedDate = DateTime.UtcNow;
                _cartItemRepository.Update(existingItem);
            }
            else
            {
                // Create new cart item
                var newItem = await CreateCartItemAsync(cart.Id, variant, product, dto.Quantity);
                cart.CartItems.Add(newItem);
                await _cartItemRepository.AddAsync(newItem);
            }

            // Recalculate totals
            RecalculateCartTotals(cart);
            _cartRepository.Update(cart);

            await _cartRepository.SaveChangesAsync();

            return MapToDto(cart);
        }

        public async Task<ShoppingCartDto> UpdateCartItemAsync(UpdateCartItemDto dto, int? customerId, string? sessionId)
        {
            var cart = await GetCartAsync(customerId, sessionId);
            if (cart == null)
                throw new InvalidOperationException("Cart not found");

            var item = cart.CartItems.FirstOrDefault(i => i.Id == dto.CartItemId);
            if (item == null)
                throw new InvalidOperationException("Cart item not found");

            if (dto.Quantity == 0)
            {
                // Remove item
                cart.CartItems.Remove(item);
                _cartItemRepository.Delete(item);
            }
            else
            {
                // Update quantity
                item.Quantity = dto.Quantity;
                item.TotalLineAmount = item.Quantity * item.UnitPrice;
                item.UpdatedDate = DateTime.UtcNow;
                _cartItemRepository.Update(item);
            }

            // Recalculate totals
            RecalculateCartTotals(cart);
            _cartRepository.Update(cart);

            await _cartRepository.SaveChangesAsync();

            return MapToDto(cart);
        }

        public async Task<ShoppingCartDto> RemoveCartItemAsync(int cartItemId, int? customerId, string? sessionId)
        {
            var cart = await GetCartAsync(customerId, sessionId);
            if (cart == null)
                throw new InvalidOperationException("Cart not found");

            var item = cart.CartItems.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null)
                throw new InvalidOperationException("Cart item not found");

            cart.CartItems.Remove(item);
            _cartItemRepository.Delete(item);

            // Recalculate totals
            RecalculateCartTotals(cart);
            _cartRepository.Update(cart);

            await _cartRepository.SaveChangesAsync();

            return MapToDto(cart);
        }

        public async Task RemoveCartItemsAsync(HashSet<int> cartItemIds, int? customerId, string? sessionId)
        {
            var cart = await GetCartAsync(customerId, sessionId);
            if (cart == null)
                throw new InvalidOperationException("Cart not found");

            if (cart.CartItems.All(x => cartItemIds.Contains(x.Id)))
            {
                await ClearCartAsync(customerId, sessionId);
                return;
            }

            var items = cart.CartItems.Where(i => cartItemIds.Contains(i.Id)).ToList();
            if (items == null || !items.Any())
                throw new InvalidOperationException("Cart item not found");

            foreach(var item in items)
            {
                cart.CartItems.Remove(item);
                _cartItemRepository.Delete(item);
            }            

            // Recalculate totals
            RecalculateCartTotals(cart);
            _cartRepository.Update(cart);

            await _cartRepository.SaveChangesAsync();

            return;
        }

        public async Task ClearCartAsync(int? customerId, string? sessionId)
        {
            var cart = await GetCartAsync(customerId, sessionId);
            if (cart == null)
                return;

            _cartItemRepository.DeleteRange(cart.CartItems);
            cart.CartItems.Clear();

            RecalculateCartTotals(cart);
            _cartRepository.Update(cart);

            await _cartRepository.SaveChangesAsync();
        }

        #region Private Helper Methods

        private async Task<ShoppingCart> GetOrCreateCartAsync(int? customerId, string? sessionId)
        {
            ShoppingCart? cart = null;

            if (customerId.HasValue)
            {
                cart = await _cartRepository.GetCartWithItemsByCustomerIdAsync(customerId.Value);
            }
            else if (!string.IsNullOrEmpty(sessionId))
            {
                cart = await _cartRepository.GetCartWithItemsBySessionIdAsync(sessionId);
            }

            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    CustomerId = customerId,
                    SessionId = sessionId,
                    CreatedDate = DateTime.UtcNow
                };
                await _cartRepository.AddAsync(cart);
                await _cartRepository.SaveChangesAsync();
            }

            return cart;
        }

        private async Task<ShoppingCart?> GetCartAsync(int? customerId, string? sessionId)
        {
            if (customerId.HasValue)
            {
                return await _cartRepository.GetCartWithItemsByCustomerIdAsync(customerId.Value);
            }
            else if (!string.IsNullOrEmpty(sessionId))
            {
                return await _cartRepository.GetCartWithItemsBySessionIdAsync(sessionId);
            }

            return null;
        }

        private async Task MergeLocalCartItemsAsync(ShoppingCart cart, List<LocalCartItemDto> localItems)
        {
            foreach (var localItem in localItems)
            {
                // Validate variant exists and is active
                var variant = await _variantRepository.GetByIdAsync(localItem.ProductVariantId);
                if (variant == null || !variant.IsActive) continue;

                var product = await _productRepository.GetByIdAsync(variant.ProductId);
                if (product == null || !product.IsActive) continue;

                // Check if item already exists in cart
                var existingItem = cart.CartItems.FirstOrDefault(i => i.ProductVariantId == localItem.ProductVariantId);

                if (existingItem != null)
                {
                    // Add to existing quantity
                    existingItem.Quantity += localItem.Quantity;
                    existingItem.TotalLineAmount = existingItem.Quantity * existingItem.UnitPrice;
                    existingItem.UpdatedDate = DateTime.UtcNow;
                    _cartItemRepository.Update(existingItem);
                }
                else
                {
                    // Create new cart item with snapshot data
                    var newItem = await CreateCartItemAsync(cart.Id, variant, product, localItem.Quantity);
                    cart.CartItems.Add(newItem);
                    await _cartItemRepository.AddAsync(newItem);
                }
            }
        }

        private async Task<ShoppingCartItem> CreateCartItemAsync(int cartId, ProductVariant variant, Product product, int quantity)
        {
            // Build variant attributes string from VariantOptionValues
            var variantAttributes = string.Empty;
            // TODO: Load and build variant attributes if needed

            return new ShoppingCartItem
            {
                ShoppingCartId = cartId,
                ProductId = variant.ProductId,
                ProductVariantId = variant.Id,
                Quantity = quantity,
                UnitPrice = variant.Price,
                OriginalPrice = variant.OriginalPrice,
                TotalLineAmount = quantity * variant.Price,
                ProductName = product.Name,
                VariantSKU = variant.SKU,
                ImageUrl = variant.ImageUrl ?? product.ThumbnailUrl,
                VariantAttributes = variantAttributes,
                IsAvailable = true,
                CurrentStockQuantity = variant.StockQuantity,
                PriceChanged = false,
                CreatedDate = DateTime.UtcNow
            };
        }

        private async Task ValidateCartItemsAsync(ShoppingCart cart)
        {
            foreach (var item in cart.CartItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                var variant = await _variantRepository.GetByIdAsync(item.ProductVariantId);

                // Check active status
                item.IsAvailable = product?.IsActive == true
                                && variant?.IsActive == true
                                && variant?.StockQuantity >= item.Quantity;

                // Update stock info
                item.CurrentStockQuantity = variant?.StockQuantity;

                // Check price change (don't auto-update)
                if (variant != null && variant.Price != item.UnitPrice)
                {
                    item.PriceChanged = true;
                }

                _cartItemRepository.Update(item);
            }
        }

        private void RecalculateCartTotals(ShoppingCart cart)
        {
            cart.TotalItems = cart.CartItems.Sum(i => i.Quantity);
            cart.SubTotal = cart.CartItems.Sum(i => i.TotalLineAmount);
            cart.TotalAmount = cart.SubTotal; // No discount
            cart.UpdatedDate = DateTime.UtcNow;
        }

        private ShoppingCartDto MapToDto(ShoppingCart cart)
        {
            return new ShoppingCartDto
            {
                Id = cart.Id,
                CustomerId = cart.CustomerId,
                SessionId = cart.SessionId,
                TotalItems = cart.TotalItems,
                SubTotal = cart.SubTotal,
                TotalAmount = cart.TotalAmount,
                Note = cart.Note,
                CreatedDate = cart.CreatedDate,
                UpdatedDate = cart.UpdatedDate,
                CartItems = cart.CartItems.Select(i => new ShoppingCartItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductVariantId = i.ProductVariantId,
                    ProductName = i.ProductName,
                    VariantSKU = i.VariantSKU,
                    VariantAttributes = i.VariantAttributes,
                    ImageUrl = i.ImageUrl,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    OriginalPrice = i.OriginalPrice,
                    TotalLineAmount = i.TotalLineAmount,
                    IsAvailable = i.IsAvailable,
                    CurrentStockQuantity = i.CurrentStockQuantity,
                    PriceChanged = i.PriceChanged,
                    CreatedDate = i.CreatedDate,
                    UpdatedDate = i.UpdatedDate,
                    BrandId = i.Product?.BrandId,
                    BrandName = i.Product?.Brand?.Name
                }).ToList()
            };
        }

        #endregion
    }
}
