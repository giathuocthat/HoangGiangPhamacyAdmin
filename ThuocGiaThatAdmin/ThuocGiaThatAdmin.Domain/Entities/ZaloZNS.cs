using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class ZaloZNS : AuditableEntity
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string ZaloUserId { get; set; }
        public string Phone { get; set; }
        public string MessageId { get; set; }
        public string Type { get; set; }
        public string TemplateId { get; set; }
        public string TemplateData { get; set; }
        public bool IsActive { get; set; } 
        public DateTime RequestTime { get; set; }
        public DateTime SentTime { get; set; }
    }
}
