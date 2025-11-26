using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// ProductType entity - Lo?i s?n ph?m (Thu?c, Vitamin, Ch?t b? sung, v.v.)
    /// </summary>
    public class ProductType
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Tên lo?i s?n ph?m (Thu?c, Vitamin, Ch?t b? sung)
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Mô t? lo?i s?n ph?m
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// ???ng d?n SEO URL slug
        /// </summary>
        public string Slug { get; set; } = string.Empty;
        
        /// <summary>
        /// Th? t? hi?n th?
        /// </summary>
        public int DisplayOrder { get; set; } = 0;
        
        /// <summary>
        /// Tr?ng thái ho?t ??ng
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Ngày t?o
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Ngày c?p nh?t
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        // Navigation properties
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

