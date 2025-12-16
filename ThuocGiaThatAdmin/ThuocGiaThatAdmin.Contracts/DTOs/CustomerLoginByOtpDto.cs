using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    public class CustomerLoginByOtpDto
    {
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Otp is required")]
        [StringLength(4)]
        public string Otp { get; set; }
    }
}
