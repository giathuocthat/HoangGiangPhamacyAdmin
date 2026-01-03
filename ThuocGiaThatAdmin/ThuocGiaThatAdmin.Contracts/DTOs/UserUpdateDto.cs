using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    public class UserUpdateDto
    {
        [StringLength(200)]
        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public string? Role { get; set; }

        public int? DepartmentId { get; set; }

        public string? AvatarUrl { get; set; }

        public string? Email { get; set; }
        }
}
