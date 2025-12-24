using System;
using System.Collections.Generic;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    /// <summary>
    /// Represents a sales region/territory (e.g., Miền Bắc, Miền Trung, Miền Nam)
    /// </summary>
    public class SalesRegion : AuditableEntity
    {
        /// <summary>
        /// Tên vùng (e.g., "Miền Tây", "Miền Đông")
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Mã vùng (e.g., "MW", "MD", "MT") - unique
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả về vùng
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Trạng thái hoạt động
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// ID của Sales Manager quản lý vùng này
        /// </summary>
        public string? SalesManagerId { get; set; }

        // ========== Navigation Properties ==========

        /// <summary>
        /// Sales Manager quản lý vùng này
        /// </summary>
        public ApplicationUser? SalesManager { get; set; }

        /// <summary>
        /// Danh sách Sales Users thuộc vùng này
        /// </summary>
        public ICollection<ApplicationUser> SalesUsers { get; set; } = new List<ApplicationUser>();

        /// <summary>
        /// Danh sách Customers thuộc vùng này
        /// </summary>
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}
