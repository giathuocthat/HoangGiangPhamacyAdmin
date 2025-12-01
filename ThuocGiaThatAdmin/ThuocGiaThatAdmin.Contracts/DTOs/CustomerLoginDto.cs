using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    public class CustomerLoginDto
    {
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
