using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Lịch sử xác minh/duyệt khách hàng
    /// </summary>
    public class CustomerVerification : AuditableEntity
    {
        // ========== Basic Information ==========
        // Id inherited from BaseEntity
        
        /// <summary>
        /// ID khách hàng
        /// </summary>
        public int CustomerId { get; set; }
        
        /// <summary>
        /// Trạng thái cũ (trước khi thay đổi)
        /// </summary>
        public CustomerStatus OldStatus { get; set; }
        
        /// <summary>
        /// Trạng thái mới (sau khi thay đổi)
        /// </summary>
        public CustomerStatus NewStatus { get; set; }
        
        /// <summary>
        /// ID nhân viên thực hiện duyệt/thay đổi trạng thái
        /// </summary>
        public string? ProcessedByUserId { get; set; }
        
        /// <summary>
        /// Thời gian xử lý
        /// </summary>
        public DateTime ProcessedDate { get; set; } = DateTime.UtcNow;
        
        // ========== Verification Details ==========
        
        /// <summary>
        /// Ghi chú/lý do thay đổi trạng thái
        /// </summary>
        public string? Notes { get; set; }
        
        /// <summary>
        /// Lý do từ chối (nếu NewStatus = Rejected)
        /// </summary>
        public string? RejectionReason { get; set; }
        
        /// <summary>
        /// Danh sách giấy tờ cần bổ sung (JSON array)
        /// Ví dụ: ["Giấy phép kinh doanh", "Chứng chỉ hành nghề"]
        /// </summary>
        public string? RequiredDocuments { get; set; }
        
        /// <summary>
        /// Điểm đánh giá hồ sơ (1-5)
        /// </summary>
        public int? Rating { get; set; }
        
        /// <summary>
        /// Có phải là lần duyệt đầu tiên không
        /// </summary>
        public bool IsInitialApproval { get; set; } = false;
        
        /// <summary>
        /// IP address của người thực hiện
        /// </summary>
        public string? IpAddress { get; set; }
        
        /// <summary>
        /// User Agent
        /// </summary>
        public string? UserAgent { get; set; }
        
        // ========== Navigation Properties ==========
        
        /// <summary>
        /// Khách hàng được xác minh
        /// </summary>
        public Customer Customer { get; set; } = null!;
        
        /// <summary>
        /// Nhân viên xử lý
        /// </summary>
        public ApplicationUser? ProcessedByUser { get; set; }
    }
}
