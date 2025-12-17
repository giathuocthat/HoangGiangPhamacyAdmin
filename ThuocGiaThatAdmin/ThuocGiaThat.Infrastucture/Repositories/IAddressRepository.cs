using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.Responses;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public interface IAddressRepository : IRepository<Address>
    {
        new Task<AddressDetailResponse?> GetByIdAsync(int id);
        Task<List<AddressListItemDto>> GetByCustomerIdAsync(int customerId);
        Task<AddressListItemDto?> GetDefaultByCustomerIdAsync(int customerId);
        Task<Address> CreateAsync(Address address);
        Task<Address> UpdateAsync(Address address);
        Task DeleteAsync(int id);
        Task SetDefaultAsync(int id, int customerId);
    }
}
