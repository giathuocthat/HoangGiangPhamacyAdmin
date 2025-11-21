using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public abstract class AuditableEntity : BaseEntity
    {
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
    }
}
