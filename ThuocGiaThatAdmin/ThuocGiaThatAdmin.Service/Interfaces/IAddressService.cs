using ThuocGiaThatAdmin.Contract.DTOs;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface IAddressService
    {
        Task<AddressDto> GetDefaultAddress(int customerId);
    }
}
