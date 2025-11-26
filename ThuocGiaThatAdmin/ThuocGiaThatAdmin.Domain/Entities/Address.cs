using System;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class Address : AuditableEntity
    {
        // Id inherited
        public int CustomerId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string? Ward { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
        public bool IsDefault { get; set; } = false;
        // Dates inherited

        public Customer Customer { get; set; } = null!;
    }
}
