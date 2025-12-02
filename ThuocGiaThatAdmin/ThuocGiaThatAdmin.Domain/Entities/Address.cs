using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class Address : AuditableEntity
    {
        // Id inherited
        public int CustomerId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public int? WardId { get; set; }
        public int? ProvinceId { get; set; }
        public bool IsDefault { get; set; } = false;
        public int? AddressType { get; set; } // 0: Home, 1: Office, null: Others
        
        // Navigation Properties
        public Customer Customer { get; set; } = null!;
        public Province? Province { get; set; }
        public Ward? Ward { get; set; }
    }
}
