using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    // ========== Request DTOs ==========
    
    /// <summary>
    /// DTO để thêm sản phẩm vào giỏ hàng
    /// </summary>
    public class AddToCartDto
    {
        [Required]
        public int ProductVariantId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; } = 1;
        
        /// <summary>
        /// Session ID cho khách chưa đăng nhập
        /// </summary>
        public string? SessionId { get; set; }
    }
    
    /// <summary>
    /// DTO để cập nhật số lượng sản phẩm trong giỏ
    /// </summary>
    public class UpdateCartItemDto
    {
        [Required]
        public int CartItemId { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không được âm")]
        public int Quantity { get; set; }
    }
    
    /// <summary>
    /// DTO để merge local cart (từ frontend) vào cart chính khi load cart
    /// </summary>
    public class MergeLocalCartDto
    {
        /// <summary>
        /// Session ID cho khách chưa đăng nhập
        /// </summary>
        public string? SessionId { get; set; }
        
        /// <summary>
        /// Danh sách items từ local storage
        /// </summary>
        public List<LocalCartItemDto> LocalCartItems { get; set; } = new List<LocalCartItemDto>();
    }
    
    /// <summary>
    /// DTO cho item từ local cart
    /// </summary>
    public class LocalCartItemDto
    {
        [Required]
        public int ProductVariantId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
    
    // ========== Response DTOs ==========
    
    /// <summary>
    /// DTO response cho shopping cart item
    /// </summary>
    public class ShoppingCartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string VariantSKU { get; set; } = string.Empty;
        public string? VariantAttributes { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal TotalLineAmount { get; set; }
        public bool IsAvailable { get; set; }
        public int? CurrentStockQuantity { get; set; }
        public bool PriceChanged { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? BrandId { get; set; }
        public string? BrandName { get; set; }
    }
    
    /// <summary>
    /// DTO response cho shopping cart đầy đủ
    /// </summary>
    public class ShoppingCartDto
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string? SessionId { get; set; }
        public int TotalItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        
        public List<ShoppingCartItemDto> CartItems { get; set; } = new List<ShoppingCartItemDto>();
    }
    
    /// <summary>
    /// DTO response đơn giản cho cart summary (hiển thị trên header)
    /// </summary>
    public class CartSummaryDto
    {
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
        public int CartId { get; set; }
    }
}
