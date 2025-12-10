using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class IPNLog : AuditableEntity
    {
        public string TransactionCode { get; set; }
        public string RawData { get; set; }
        public Boolean IsVerfied { get; set; }
        public Boolean Processed { get; set; }
    }
}
