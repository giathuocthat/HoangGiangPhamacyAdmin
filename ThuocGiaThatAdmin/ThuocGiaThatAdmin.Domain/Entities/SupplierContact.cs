using System;
using ThuocGiaThatAdmin.Domain.Enums;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Người liên hệ của nhà cung cấp
    /// </summary>
    public class SupplierContact : AuditableEntity
    {
        // Id inherited from AuditableEntity

        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public int SupplierId { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Chức vụ
        /// </summary>
        public string? Position { get; set; }

        /// <summary>
        /// Phòng ban
        /// </summary>
        public string? Department { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Số điện thoại cố định
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Số điện thoại di động
        /// </summary>
        public string? Mobile { get; set; }

        /// <summary>
        /// Loại liên hệ
        /// </summary>
        public SupplierContactType ContactType { get; set; } = SupplierContactType.Secondary;

        /// <summary>
        /// Trạng thái hoạt động
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Là liên hệ chính
        /// </summary>
        public bool IsPrimary { get; set; } = false;

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Notes { get; set; }

        // Navigation properties
        public Supplier Supplier { get; set; } = null!;
    }
}
