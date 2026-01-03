using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    public class UserCreateDto
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;

        [StringLength(200)]
        public string? FullName { get; set; }

        public string? Phone { get; set; }
        public string? Role { get; set; }

        public int? DepartmentId { get; set; }

        public string? AvatarUrl { get; set; }
    }
}
