using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Represents a department/division within the organization
    /// </summary>
    public class Department : AuditableEntity
    {
        /// <summary>
        /// Tên phòng ban (e.g., "Phòng Kinh Doanh", "Phòng Kế Toán")
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Mã phòng ban (e.g., "KB", "KT", "HC") - unique
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả về phòng ban
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Trạng thái hoạt động
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// ID của người quản lý phòng ban (Department Manager)
        /// </summary>
        public string? ManagerId { get; set; }

        // ========== Navigation Properties ==========

        /// <summary>
        /// Người quản lý phòng ban
        /// </summary>
        public ApplicationUser? Manager { get; set; }

        /// <summary>
        /// Danh sách nhân viên thuộc phòng ban này
        /// </summary>
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        /// <summary>
        /// Danh sách roles được định nghĩa cho phòng ban này
        /// </summary>
        public ICollection<DepartmentRole> DepartmentRoles { get; set; } = new List<DepartmentRole>();
    }
}
