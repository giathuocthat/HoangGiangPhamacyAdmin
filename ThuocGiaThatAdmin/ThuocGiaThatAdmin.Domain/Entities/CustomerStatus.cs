using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Trạng thái tài khoản khách hàng
    /// Trạng thái được tự động xác định dựa trên việc duyệt các document
    /// </summary>
    public enum CustomerStatus
    {
        /// <summary>
        /// Chờ duyệt - Khách hàng mới đăng ký hoặc chưa có đủ giấy tờ được xác minh
        /// Có thể đăng nhập nhưng bị giới hạn chức năng
        /// </summary>
        PendingApproval = 0,
        
        /// <summary>
        /// Đã duyệt - Tất cả giấy tờ bắt buộc đã được xác minh
        /// Có thể sử dụng đầy đủ chức năng
        /// </summary>
        Approved = 1,
        
        /// <summary>
        /// Tạm khóa - Tài khoản bị tạm khóa do vi phạm hoặc lý do khác
        /// Không thể đăng nhập
        /// </summary>
        Suspended = 2
    }
}
