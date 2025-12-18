using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Contract.Responses
{
    public class AddressDetailResponse
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
        public string FullAddress => $"{AddressLine}, {WardType} {WardName}, {ProvinceType} {ProvinceName}";
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? ProvinceType { get; set; }
        public string? WardType { get; set; }
    }
}
