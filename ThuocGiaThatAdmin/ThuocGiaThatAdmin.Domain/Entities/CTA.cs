using System;
using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// CTA (Call To Action) - Nút kêu gọi hành động
    /// </summary>
    public class CTA : AuditableEntity
    {
        /// <summary>
        /// Mã CTA (unique identifier)
        /// </summary>
        [MaxLength(50)]
        public string? Code { get; set; }

        /// <summary>
        /// Tên CTA (cho quản lý nội bộ)
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Tiêu đề hiển thị
        /// </summary>
        [MaxLength(500)]
        public string? Title { get; set; }

        /// <summary>
        /// Mô tả ngắn
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Text hiển thị trên button/link
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string ButtonText { get; set; } = string.Empty;

        /// <summary>
        /// Loại CTA
        /// </summary>
        public CTAType Type { get; set; }

        /// <summary>
        /// Vị trí hiển thị
        /// </summary>
        public CTAPosition Position { get; set; }

        /// <summary>
        /// Style của button
        /// </summary>
        public CTAStyle Style { get; set; }

        /// <summary>
        /// URL đích khi click
        /// </summary>
        [MaxLength(1000)]
        public string? TargetUrl { get; set; }

        /// <summary>
        /// Mở link trong tab mới
        /// </summary>
        public bool OpenInNewTab { get; set; }

        /// <summary>
        /// Icon (class name hoặc URL)
        /// </summary>
        [MaxLength(200)]
        public string? Icon { get; set; }

        /// <summary>
        /// URL hình ảnh (nếu là banner/card)
        /// </summary>
        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// URL hình ảnh cho mobile
        /// </summary>
        [MaxLength(500)]
        public string? MobileImageUrl { get; set; }

        /// <summary>
        /// Màu nền tùy chỉnh
        /// </summary>
        [MaxLength(50)]
        public string? BackgroundColor { get; set; }

        /// <summary>
        /// Màu chữ tùy chỉnh
        /// </summary>
        [MaxLength(50)]
        public string? TextColor { get; set; }

        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Ngày bắt đầu hiển thị
        /// </summary>
        public DateTime? ValidFrom { get; set; }

        /// <summary>
        /// Ngày kết thúc hiển thị
        /// </summary>
        public DateTime? ValidTo { get; set; }

        /// <summary>
        /// Kích hoạt
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Số lần hiển thị
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// Số lần click
        /// </summary>
        public int ClickCount { get; set; }

        /// <summary>
        /// Campaign liên kết (nếu có)
        /// </summary>
        public int? CampaignId { get; set; }

        /// <summary>
        /// Metadata bổ sung (JSON)
        /// </summary>
        public string? Metadata { get; set; }

        // Navigation Properties
        public virtual Campaign? Campaign { get; set; }
    }
}
