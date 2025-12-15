using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class BannerAnalytics : AuditableEntity
    {
        public int BannerId { get; set; }

        public int? CustomerId { get; set; }

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }

        public string? DeviceType { get; set; }

        public int EventType { get; set; } // 0: View, 1: Click

        // Navigation Properties
        public virtual Banner Banner { get; set; }
       
    }
}
