using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface IShoppingCartService
    {
        /// <summary>
        /// Load cart with optional local cart merge
        /// </summary>
        Task<ShoppingCartDto> LoadCartAsync(int? customerId, string? sessionId, MergeLocalCartDto? localCart = null);
        
        /// <summary>
        /// Get cart summary for header display
        /// </summary>
        Task<CartSummaryDto> GetCartSummaryAsync(int? customerId, string? sessionId);
        
        /// <summary>
        /// Add item to cart
        /// </summary>
        Task<ShoppingCartDto> AddToCartAsync(AddToCartDto dto, int? customerId);
        
        /// <summary>
        /// Update cart item quantity
        /// </summary>
        Task<ShoppingCartDto> UpdateCartItemAsync(UpdateCartItemDto dto, int? customerId, string? sessionId);
        
        /// <summary>
        /// Remove item from cart
        /// </summary>
        Task<ShoppingCartDto> RemoveCartItemAsync(int cartItemId, int? customerId, string? sessionId);
        
        /// <summary>
        /// Clear entire cart
        /// </summary>
        Task ClearCartAsync(int? customerId, string? sessionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cartItemIds"></param>
        /// <param name="customerId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        Task RemoveCartItemsAsync(HashSet<int> cartItemIds, int? customerId, string? sessionId);
    }
}
