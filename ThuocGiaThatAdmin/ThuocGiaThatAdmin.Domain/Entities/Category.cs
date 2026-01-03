using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Category entity - Danh mục sản phẩm (hỗ trợ cấu trúc cha-con)
    /// </summary>
    public class Category : AuditableEntity
    {
        // Id inherited from BaseEntity

        /// <summary>
        /// Tên danh mục
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả danh mục
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// ID danh mục cha (null nếu là danh mục gốc)
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Đường dẫn SEO URL slug
        /// </summary>
        public string Slug { get; set; } = string.Empty;

        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int DisplayOrder { get; set; } = 0;

        /// <summary>
        /// Trạng thái hoạt động
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Dates inherited

        // Navigation properties
        public Category? ParentCategory { get; set; }
        public ICollection<Category> ChildCategories { get; set; } = new List<Category>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public string? ImageUrl { get; set; }
    }
}
