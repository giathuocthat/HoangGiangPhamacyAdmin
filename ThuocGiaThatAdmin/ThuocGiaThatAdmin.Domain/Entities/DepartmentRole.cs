using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Represents the many-to-many relationship between Department and ApplicationRole.
    /// Defines which roles are available/applicable for each department.
    /// </summary>
    public class DepartmentRole : AuditableEntity
    {
        /// <summary>
        /// ID của phòng ban
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// ID của role
        /// </summary>
        public string RoleId { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả về role trong context của phòng ban này (optional)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Trạng thái hoạt động của role trong phòng ban này
        /// </summary>
        public bool IsActive { get; set; } = true;

        // ========== Navigation Properties ==========

        /// <summary>
        /// Navigation property đến Department
        /// </summary>
        public Department Department { get; set; } = null!;

        /// <summary>
        /// Navigation property đến ApplicationRole
        /// </summary>
        public ApplicationRole Role { get; set; } = null!;
    }
}
