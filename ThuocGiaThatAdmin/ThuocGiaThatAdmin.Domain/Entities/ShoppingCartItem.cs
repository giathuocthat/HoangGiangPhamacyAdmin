using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// ShoppingCartItem entity - Sản phẩm trong giỏ hàng
    /// </summary>
    public class ShoppingCartItem : AuditableEntity
    {
        // Id inherited from BaseEntity
        
        // ========== References ==========
        /// <summary>
        /// ID của giỏ hàng
        /// </summary>
        public int ShoppingCartId { get; set; }
        
        /// <summary>
        /// ID của sản phẩm
        /// </summary>
        public int ProductId { get; set; }
        
        /// <summary>
        /// ID của biến thể sản phẩm
        /// </summary>
        public int ProductVariantId { get; set; }
        
        // ========== Quantity ==========
        /// <summary>
        /// Số lượng sản phẩm
        /// </summary>
        public int Quantity { get; set; } = 1;
        
        // ========== Price Snapshot ==========
        /// <summary>
        /// Giá tại thời điểm thêm vào giỏ (snapshot)
        /// </summary>
        public decimal UnitPrice { get; set; }
        
        /// <summary>
        /// Giá gốc (nếu có sale)
        /// </summary>
        public decimal? OriginalPrice { get; set; }
        
        /// <summary>
        /// Tổng tiền cho item này (Quantity * UnitPrice)
        /// </summary>
        public decimal TotalLineAmount { get; set; }
        
        // ========== Product Info Snapshot ==========
        /// <summary>
        /// Tên sản phẩm (snapshot để hiển thị nhanh)
        /// </summary>
        public string ProductName { get; set; } = string.Empty;
        
        /// <summary>
        /// SKU của variant (snapshot)
        /// </summary>
        public string VariantSKU { get; set; } = string.Empty;
        
        /// <summary>
        /// Hình ảnh sản phẩm (snapshot)
        /// </summary>
        public string? ImageUrl { get; set; }
        
        /// <summary>
        /// Thuộc tính variant (VD: "Hộp 30 viên", "Chai 100ml")
        /// </summary>
        public string? VariantAttributes { get; set; }
        
        // ========== Stock Validation ==========
        /// <summary>
        /// Có còn hàng không (checked khi load giỏ hàng)
        /// </summary>
        public bool IsAvailable { get; set; } = true;
        
        /// <summary>
        /// Số lượng tồn kho hiện tại (để validate)
        /// </summary>
        public int? CurrentStockQuantity { get; set; }
        
        /// <summary>
        /// Cảnh báo nếu giá đã thay đổi
        /// </summary>
        public bool PriceChanged { get; set; } = false;
        
        // CreatedDate, UpdatedDate inherited from AuditableEntity
        
        // ========== Navigation Properties ==========
        public ShoppingCart ShoppingCart { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
