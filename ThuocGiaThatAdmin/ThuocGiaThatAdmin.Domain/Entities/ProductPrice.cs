using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// ProductPrice entity - L?ch s? thay ??i giá s?n ph?m
    /// </summary>
    public class ProductPrice
    {
        public int Id { get; set; }
        
        /// <summary>
        /// ID s?n ph?m
        /// </summary>
        public int ProductId { get; set; }
        
        /// <summary>
        /// Giá g?c
        /// </summary>
        public decimal CostPrice { get; set; }
        
        /// <summary>
        /// Giá bán
        /// </summary>
        public decimal SalePrice { get; set; }
        
        /// <summary>
        /// Giá khuy?n mãi
        /// </summary>
        public decimal? DiscountPrice { get; set; }
        
        /// <summary>
        /// Lý do thay ??i giá
        /// </summary>
        public string? Reason { get; set; }
        
        /// <summary>
        /// Ngày hi?u l?c
        /// </summary>
        public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Ngày k?t thúc hi?u l?c (null = hi?n t?i)
        /// </summary>
        public DateTime? EndDate { get; set; }
        
        /// <summary>
        /// Ngày t?o
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Product Product { get; set; } = null!;
    }
}
