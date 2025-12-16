using ThuocGiaThatAdmin.Contracts.DTOs;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface IAddressService
    {
        Task<AddressDto?> GetByIdAsync(int id);
        Task<List<AddressListItemDto>> GetByCustomerIdAsync(int customerId);
        Task<AddressDto?> GetDefaultAddress(int customerId);
        Task<AddressDto> CreateAsync(CreateAddressDto dto);
        Task<AddressDto> UpdateAsync(int id, UpdateAddressDto dto);
        Task DeleteAsync(int id);
        Task SetDefaultAsync(int id, int customerId);
    }
}
