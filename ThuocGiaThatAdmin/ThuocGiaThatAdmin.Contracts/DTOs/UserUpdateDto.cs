using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    public class UserUpdateDto
    {
        [StringLength(200)]
        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? SelectedRole { get; set; }
    }
}
