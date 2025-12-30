using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public string? DisplayName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        /// <summary>
        /// Danh sách phòng ban mà role này được áp dụng
        /// </summary>
        public ICollection<DepartmentRole> DepartmentRoles { get; set; } = new List<DepartmentRole>();
    }
}
