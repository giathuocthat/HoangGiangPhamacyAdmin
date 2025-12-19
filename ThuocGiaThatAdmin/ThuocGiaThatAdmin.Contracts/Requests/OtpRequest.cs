using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.Enums;

namespace ThuocGiaThatAdmin.Contract.Requests
{
    public class OtpRequest
    {
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(0[0-9]{9,10})$", ErrorMessage = "Phone number is not valid")]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]

        public OtpCodeTypeEnum Type { get; set; }
    }
}
