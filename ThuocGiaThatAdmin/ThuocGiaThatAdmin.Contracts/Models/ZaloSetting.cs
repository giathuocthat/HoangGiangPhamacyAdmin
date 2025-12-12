using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Contract.Models
{
    public class ZaloSetting
    {
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; }
        public string OAId { get; set; }
        public int ZNSTemplateId { get; set; }
    }
}
