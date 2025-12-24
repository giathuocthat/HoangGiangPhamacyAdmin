using System.ComponentModel.DataAnnotations;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    public class UpdateCustomerDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Business type is required")]
        public int BusinessTypeId { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string AddressLine { get; set; } = string.Empty;

        [Required(ErrorMessage = "Province is required")]
        public int ProvinceId { get; set; }

        [Required(ErrorMessage = "Ward is required")]
        public int WardId { get; set; }

        [StringLength(100, ErrorMessage = "Recipient name cannot exceed 100 characters")]
        public string? RecipientName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Address phone number cannot exceed 20 characters")]
        public string? AddressPhoneNumber { get; set; }

        /// <summary>
        /// ID của Sale User phụ trách khách hàng này
        /// </summary>
        public string? SaleUserId { get; set; }
    }
}
