using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// ProductImage entity - Hình ?nh s?n ph?m
    /// </summary>
    public class ProductImage
    {
        public int Id { get; set; }
        
        /// <summary>
        /// ID s?n ph?m
        /// </summary>
        public int ProductId { get; set; }
        
        /// <summary>
        /// URL hình ?nh
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Mô t? hình ?nh (alt text)
        /// </summary>
        public string? AltText { get; set; }
        
        /// <summary>
        /// Là hình ?nh ??i di?n
        /// </summary>
        public bool IsPrimary { get; set; } = false;
        
        /// <summary>
        /// Th? t? hi?n th?
        /// </summary>
        public int DisplayOrder { get; set; } = 0;
        
        /// <summary>
        /// Ngày t?o
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Product Product { get; set; } = null!;
    }
}
