using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    public class UpdateCustomerProfileDto
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }


        [EmailAddress]
        [StringLength(20)]
        public string Email { get; set; }
    }
}
