using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    public class CustomerProfileTokenDto : CustomerProfileDto
    {
        public string Token { get; set; }
        public string ExpiresAt { get; set; }
    }
}
