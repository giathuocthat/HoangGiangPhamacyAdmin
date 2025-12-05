using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    public class UserDto
    {
        public string Id { get; set; } = null!;
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string[] Roles { get; set; } = System.Array.Empty<string>();
        public string? PhoneNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; }

    }
}
