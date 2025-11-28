using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// ShoppingCart entity - Giỏ hàng
    /// Lưu trữ giỏ hàng của khách hàng (cả đã đăng nhập và chưa đăng nhập)
    /// </summary>
    public class ShoppingCart : AuditableEntity
    {
        // Id inherited from BaseEntity
        
        // ========== Customer Information ==========
        /// <summary>
        /// ID khách hàng (null nếu khách chưa đăng nhập)
        /// </summary>
        public int? CustomerId { get; set; }
        
        /// <summary>
        /// Session ID cho khách chưa đăng nhập (GUID)
        /// </summary>
        public string? SessionId { get; set; }
        
        // ========== Pricing Summary (Calculated) ==========
        /// <summary>
        /// Tổng số lượng sản phẩm trong giỏ
        /// </summary>
        public int TotalItems { get; set; } = 0;
        
        /// <summary>
        /// Tổng tiền chưa giảm giá
        /// </summary>
        public decimal SubTotal { get; set; } = 0;
        
        /// <summary>
        /// Tổng tiền (= SubTotal, không có discount)
        /// </summary>
        public decimal TotalAmount { get; set; } = 0;
        
        // ========== Additional Info ==========
        /// <summary>
        /// Ghi chú của khách hàng
        /// </summary>
        public string? Note { get; set; }
        
        // CreatedDate, UpdatedDate inherited from AuditableEntity
        
        // ========== Navigation Properties ==========
        public Customer? Customer { get; set; }
        public ICollection<ShoppingCartItem> CartItems { get; set; } = new List<ShoppingCartItem>();
    }
}
