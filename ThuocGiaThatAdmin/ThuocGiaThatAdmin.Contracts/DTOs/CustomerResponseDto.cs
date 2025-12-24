using System;
using ThuocGiaThatAdmin.Contracts.DTOs;

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

        /// <summary>
        /// ID của Sale User phụ trách khách hàng này
        /// </summary>
        public string? SaleUserId { get; set; }

        /// <summary>
        /// Tên của Sale User phụ trách
        /// </summary>
        public string? SaleUserName { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
