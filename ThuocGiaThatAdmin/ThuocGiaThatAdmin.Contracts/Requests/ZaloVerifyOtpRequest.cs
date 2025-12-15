using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Contract.Requests
{
    public class ZaloVerifyOtpRequest
    {
        [Required]
        public string Phone { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 4)]
        public string OtpCode { get; set; }
    }
}
