using System;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    public class CustomerProfileDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        
        // Business Type Info (if exists)
        public int? BusinessTypeId { get; set; }
        public string? BusinessTypeName { get; set; }
        
        // Flags
        public bool HasBusinessInfo { get; set; }
        public bool HasPaymentAccounts { get; set; }
        
        public DateTime CreatedDate { get; set; }
    }
}
