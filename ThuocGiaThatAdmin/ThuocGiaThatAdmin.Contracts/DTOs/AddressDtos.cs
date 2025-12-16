using System;

namespace ThuocGiaThatAdmin.Contracts.DTOs
{
    // ============ Address DTOs ============

    public class AddressDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public int? WardId { get; set; }
        public string? WardName { get; set; }
        public int? ProvinceId { get; set; }
        public string? ProvinceName { get; set; }
        public bool IsDefault { get; set; }
        public int? AddressType { get; set; } // 0: Home, 1: Office, null: Others
        public string AddressTypeName => AddressType switch
        {
            0 => "Nhà riêng",
            1 => "Văn phòng",
            _ => "Khác"
        };
        public string FullAddress => $"{AddressLine}, {WardName}, {ProvinceName}";
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class CreateAddressDto
    {
        public int CustomerId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public int? WardId { get; set; }
        public int? ProvinceId { get; set; }
        public bool IsDefault { get; set; } = false;
        public int? AddressType { get; set; }
    }

    public class UpdateAddressDto
    {
        public string RecipientName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public int? WardId { get; set; }
        public int? ProvinceId { get; set; }
        public bool IsDefault { get; set; }
        public int? AddressType { get; set; }
    }

    public class AddressListItemDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public string? WardName { get; set; }
        public string? ProvinceName { get; set; }
        public bool IsDefault { get; set; }
        public string AddressTypeName => AddressType switch
        {
            0 => "Nhà riêng",
            1 => "Văn phòng",
            _ => "Khác"
        };
        public int? AddressType { get; set; }
    }
}
