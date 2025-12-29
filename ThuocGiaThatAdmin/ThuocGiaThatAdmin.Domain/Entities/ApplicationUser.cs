using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedDate { get; set; }
        public string? FullName { get; set; }
        public bool IsActive { get; set; }

        // ========== Sales Hierarchy ==========
        /// <summary>
        /// ID của Sale Manager quản lý user này (nếu user là Sale Member)
        /// </summary>
        public string? ManagerId { get; set; }

        /// <summary>
        /// Navigation property đến Sale Manager
        /// </summary>
        public ApplicationUser? Manager { get; set; }

        /// <summary>
        /// Danh sách Sale Members mà user này quản lý (nếu user là Sale Manager)
        /// </summary>
        public ICollection<ApplicationUser> SalesTeamMembers { get; set; } = new List<ApplicationUser>();

        /// <summary>
        /// Danh sách Customers được assign cho user này (nếu user là Sale Member)
        /// </summary>
        public ICollection<Customer> AssignedCustomers { get; set; } = new List<Customer>();

        // ========== Sales Region ==========
        /// <summary>
        /// ID của Sales Region mà user này thuộc về
        /// </summary>
        public int? RegionId { get; set; }

        /// <summary>
        /// Navigation property đến Sales Region
        /// </summary>
        public SalesRegion? Region { get; set; }

        // ========== Department ==========
        /// <summary>
        /// ID của phòng ban mà user này thuộc về
        /// </summary>
        public int? DepartmentId { get; set; }

        /// <summary>
        /// Navigation property đến Department
        /// </summary>
        public Department? Department { get; set; }
    }
}
