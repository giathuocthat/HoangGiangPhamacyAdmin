using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Contract.Requests
{
    public class CheckCustomerExistsRequest
    {
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20)]
        public string PhoneNumber { get; set; }
    }
}
