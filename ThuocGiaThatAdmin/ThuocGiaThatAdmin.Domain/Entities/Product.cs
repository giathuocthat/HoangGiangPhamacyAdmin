using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Product entity - Sản phẩm (Thuốc, vitamin, chất bổ sung)
    /// </summary>
    public class Product
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Tên sản phẩm viết tắt hoặc tên gọi khác
        /// </summary>
        public string? ShortName { get; set; }
        
        /// <summary>
        /// Mô tả chi tiết sản phẩm
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Thành phần/Công dụng
        /// </summary>
        public string? Ingredients { get; set; }
        
        /// <summary>
        /// Hướng dẫn sử dụng
        /// </summary>
        public string? UsageInstructions { get; set; }
        
        /// <summary>
        /// Cảnh báo/Chống chỉ định
        /// </summary>
        public string? Warnings { get; set; }
        
        /// <summary>
        /// Mã SKU/Mã sản phẩm
        /// </summary>
        public string SKU { get; set; } = string.Empty;
        
        /// <summary>
        /// Mã barcode
        /// </summary>
        public string? Barcode { get; set; }
        
        /// <summary>
        /// ID danh mục
        /// </summary>
        public int CategoryId { get; set; }
        
        /// <summary>
        /// ID loại sản phẩm
        /// </summary>
        public int ProductTypeId { get; set; }
        
        /// <summary>
        /// Nhà sản xuất
        /// </summary>
        public string? Manufacturer { get; set; }
        
        /// <summary>
        /// Nước sản xuất
        /// </summary>
        public string? ManufacturingCountry { get; set; }
        
        /// <summary>
        /// Hạn sử dụng (số tháng)
        /// </summary>
        public int? ExpiryMonths { get; set; }
        
        /// <summary>
        /// Hàm lượng (ví dụ: 500mg)
        /// </summary>
        public string? Strength { get; set; }
        
        /// <summary>
        /// Đơn vị tính (Hộp, Vỉ, Chai, v.v.)
        /// </summary>
        public string Unit { get; set; } = "Hộp";
        
        /// <summary>
        /// Số lượng tồn kho
        /// </summary>
        public int StockQuantity { get; set; } = 0;
        
        /// <summary>
        /// Số lượng cảnh báo tồn kho thấp
        /// </summary>
        public int LowStockThreshold { get; set; } = 10;
        
        /// <summary>
        /// Giá gốc/Giá vốn
        /// </summary>
        public decimal CostPrice { get; set; } = 0;
        
        /// <summary>
        /// Giá bán (giá hiện tại)
        /// </summary>
        public decimal SalePrice { get; set; } = 0;
        
        /// <summary>
        /// Giá khuyến mãi
        /// </summary>
        public decimal? DiscountPrice { get; set; }
        
        /// <summary>
        /// Đường dẫn SEO URL slug
        /// </summary>
        public string Slug { get; set; } = string.Empty;
        
        /// <summary>
        /// Hình ảnh đại diện
        /// </summary>
        public string? ThumbnailImage { get; set; }
        
        /// <summary>
        /// Rating trung bình (0-5)
        /// </summary>
        public decimal Rating { get; set; } = 0;
        
        /// <summary>
        /// Số lượt bán
        /// </summary>
        public int SaleCount { get; set; } = 0;
        
        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int DisplayOrder { get; set; } = 0;
        
        /// <summary>
        /// Là sản phẩm nổi bật
        /// </summary>
        public bool IsFeatured { get; set; } = false;
        
        /// <summary>
        /// Trạng thái hoạt động
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        // Navigation properties
        public virtual Category Category { get; set; } = null!;
        public virtual ProductType ProductType { get; set; } = null!;
        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public virtual ICollection<ProductPrice> PriceHistory { get; set; } = new List<ProductPrice>();
    }
}
