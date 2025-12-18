using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Server.Extensions;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    public class ShoppingCartController : BaseApiController
    {
        private readonly IShoppingCartService _cartService;

        public ShoppingCartController(IShoppingCartService cartService, ILogger<ShoppingCartController> logger)
            : base(logger)
        {
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        }

        /// <summary>
        /// Load cart
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCart([FromQuery] string? sessionId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var customerId = GetCustomerId();
                var cart = await _cartService.LoadCartAsync(customerId, sessionId);
                return Success(cart, "Cart loaded successfully");
            }, "Get Cart");
        }

        /// <summary>
        /// Load cart with optional local cart merge
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> MergeCart([FromQuery] string? sessionId, [FromBody] MergeLocalCartDto? localCart)
        {
            return await ExecuteActionAsync(async () =>
            {
                var customerId = GetCustomerId();
                var cart = await _cartService.LoadCartAsync(customerId, sessionId, localCart);
                return Success(cart, "Cart loaded successfully");
            }, "Get Cart");
        }

        /// <summary>
        /// Get cart summary for header display
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetCartSummary([FromQuery] string? sessionId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var customerId = GetCustomerId();
                var summary = await _cartService.GetCartSummaryAsync(customerId, sessionId);
                return Success(summary);
            }, "Get Cart Summary");
        }

        /// <summary>
        /// Add item to cart
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var cart = await _cartService.AddToCartAsync(dto, User.GetCustomerId());
                return Success(cart, "Item added to cart successfully");
            }, "Add to Cart");
        }

        /// <summary>
        /// Update cart item quantity
        /// </summary>
        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, [FromBody] UpdateCartItemDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                if (id != dto.CartItemId)
                    return BadRequestResponse("Cart item ID mismatch");

                var customerId = GetCustomerId();
                var sessionId = Request.Query["sessionId"].ToString();
                var cart = await _cartService.UpdateCartItemAsync(dto, customerId, sessionId);
                return Success(cart, "Cart item updated successfully");
            }, "Update Cart Item");
        }

        /// <summary>
        /// Remove item from cart
        /// </summary>
        [HttpDelete("items/{id}")]
        public async Task<IActionResult> RemoveCartItem(int id, [FromQuery] string? sessionId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var customerId = GetCustomerId();
                var cart = await _cartService.RemoveCartItemAsync(id, customerId, sessionId);
                return Success(cart, "Item removed from cart successfully");
            }, "Remove Cart Item");
        }

        /// <summary>
        /// Remove item from cart
        /// </summary>
        [HttpDelete("multi-items")]
        public async Task<IActionResult> RemoveCartItems(HashSet<int> ids, [FromQuery] string? sessionId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var customerId = GetCustomerId();
                await _cartService.RemoveCartItemsAsync(ids, customerId, sessionId);
                return Success("Item removed from cart successfully");
            }, "Remove Cart Item");
        }

        /// <summary>
        /// Clear entire cart
        /// </summary>
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart([FromQuery] string? sessionId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var customerId = GetCustomerId();
                await _cartService.ClearCartAsync(customerId, sessionId);
                return Success("Cart cleared successfully");
            }, "Clear Cart");
        }

        #region Helper Methods

        private int? GetCustomerId()
        {
            return User.GetCustomerId();
        }

        #endregion
    }
}
