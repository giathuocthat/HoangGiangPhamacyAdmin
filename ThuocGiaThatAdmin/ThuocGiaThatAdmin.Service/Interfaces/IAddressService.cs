using ThuocGiaThatAdmin.Contract.Responses;
using ThuocGiaThatAdmin.Contracts.DTOs;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface IAddressService
    {
        Task<AddressDetailResponse?> GetByIdAsync(int id);
        Task<List<AddressListItemDto>> GetByCustomerIdAsync(int customerId);
        Task<AddressListItemDto?> GetDefaultAddress(int customerId);
        Task<AddressDetailResponse> CreateAsync(CreateAddressDto dto);
        Task<AddressDetailResponse> UpdateAsync(int id, UpdateAddressDto dto);
        Task DeleteAsync(int id);
        Task SetDefaultAsync(int id, int customerId);
    }
}
