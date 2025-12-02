using System;

namespace ThuocGiaThatAdmin.Contract.DTOs
{
    public class CustomerResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public int? BusinessTypeId { get; set; }
        public string? BusinessTypeName { get; set; }
        public AddressDto? Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class AddressDto
    {
        public int Id { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public int? WardId { get; set; }
        public string? WardName { get; set; }
        public int? ProvinceId { get; set; }
        public string? ProvinceName { get; set; }
        public bool IsDefault { get; set; }
    }
}
