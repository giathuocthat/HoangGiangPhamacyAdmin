using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class OtpCode : AuditableEntity
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Code { get; set; }
        public DateTime? ExpireTime { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Type { get; set; } = string.Empty;
    }
}
