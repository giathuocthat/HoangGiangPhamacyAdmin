using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Entity lưu trữ thông tin các file upload
    /// </summary>
    public class UploadedFile : AuditableEntity
    {
        // Id inherited from BaseEntity
        
        /// <summary>
        /// Tên file gốc khi upload
        /// </summary>
        public string OriginalFileName { get; set; } = string.Empty;
        
        /// <summary>
        /// Tên file đã được lưu trên server (unique name)
        /// </summary>
        public string StoredFileName { get; set; } = string.Empty;
        
        /// <summary>
        /// Đường dẫn đầy đủ đến file trên server
        /// </summary>
        public string FilePath { get; set; } = string.Empty;
        
        /// <summary>
        /// URL công khai để truy cập file
        /// </summary>
        public string? FileUrl { get; set; }
        
        /// <summary>
        /// Kích thước file (bytes)
        /// </summary>
        public long FileSize { get; set; }
        
        /// <summary>
        /// Loại MIME của file (image/jpeg, application/pdf, etc.)
        /// </summary>
        public string ContentType { get; set; } = string.Empty;
        
        /// <summary>
        /// Extension của file (.jpg, .pdf, .docx, etc.)
        /// </summary>
        public string FileExtension { get; set; } = string.Empty;
        
        /// <summary>
        /// Loại file: Image, Document, Video, Other
        /// </summary>
        public FileType FileType { get; set; }
        
        /// <summary>
        /// Thư mục lưu trữ (products, brands, categories, documents, etc.)
        /// </summary>
        public string? StorageFolder { get; set; }
        
        /// <summary>
        /// Nguồn gốc upload - File được upload từ form/module nào
        /// </summary>
        public UploadSource UploadSource { get; set; }
        
        /// <summary>
        /// ID của entity liên quan (ProductId, CategoryId, etc.)
        /// </summary>
        public int? RelatedEntityId { get; set; }
        
        /// <summary>
        /// ID của Vendor (nếu file thuộc về vendor cụ thể)
        /// </summary>
        public int? VendorId { get; set; }
        
        /// <summary>
        /// Mô tả hoặc ghi chú về file
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// ID của user đã upload
        /// </summary>
        public string? UploadedByUserId { get; set; }
        
        /// <summary>
        /// File có đang được sử dụng không
        /// </summary>
        public bool IsInUse { get; set; } = false;
        
        /// <summary>
        /// File có bị xóa mềm không
        /// </summary>
        public bool IsDeleted { get; set; } = false;
        
        // CreatedDate và UpdatedDate inherited from AuditableEntity
    }
}
