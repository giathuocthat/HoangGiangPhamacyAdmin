using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Giấy tờ/tài liệu của khách hàng
    /// </summary>
    public class CustomerDocument : AuditableEntity
    {
        // ========== Basic Information ==========
        // Id inherited from BaseEntity
        
        /// <summary>
        /// ID khách hàng
        /// </summary>
        public int CustomerId { get; set; }
        
        /// <summary>
        /// Loại giấy tờ
        /// </summary>
        public CustomerDocumentType DocumentType { get; set; }
        
        /// <summary>
        /// ID file đã upload (reference đến bảng UploadedFile)
        /// </summary>
        public int UploadedFileId { get; set; }
        
        // ========== Document Details ==========
        
        /// <summary>
        /// Số giấy tờ (số giấy phép, số chứng chỉ...)
        /// </summary>
        public string? DocumentNumber { get; set; }
        
        /// <summary>
        /// Ngày cấp
        /// </summary>
        public DateTime? IssueDate { get; set; }
        
        /// <summary>
        /// Ngày hết hạn
        /// </summary>
        public DateTime? ExpiryDate { get; set; }
        
        /// <summary>
        /// Nơi cấp
        /// </summary>
        public string? IssuingAuthority { get; set; }
        
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Notes { get; set; }
        
        // ========== Verification Status ==========
        
        /// <summary>
        /// Trạng thái xác minh: true = đã xác minh, false = chưa xác minh, null = đang chờ
        /// </summary>
        public bool? IsVerified { get; set; }
        
        /// <summary>
        /// ID nhân viên xác minh
        /// </summary>
        public string? VerifiedByUserId { get; set; }
        
        /// <summary>
        /// Thời gian xác minh
        /// </summary>
        public DateTime? VerifiedDate { get; set; }
        
        /// <summary>
        /// Lý do từ chối (nếu IsVerified = false)
        /// </summary>
        public string? RejectionReason { get; set; }
        
        /// <summary>
        /// Có phải giấy tờ bắt buộc không
        /// </summary>
        public bool IsRequired { get; set; } = false;
        
        /// <summary>
        /// Có bị xóa không (soft delete)
        /// </summary>
        public bool IsDeleted { get; set; } = false;
        
        // ========== Navigation Properties ==========
        
        /// <summary>
        /// Khách hàng sở hữu giấy tờ này
        /// </summary>
        public Customer Customer { get; set; } = null!;
        
        /// <summary>
        /// File đã upload
        /// </summary>
        public UploadedFile UploadedFile { get; set; } = null!;
        
        /// <summary>
        /// Nhân viên xác minh
        /// </summary>
        public ApplicationUser? VerifiedByUser { get; set; }
    }
}
