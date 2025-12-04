using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<AddressDto> GetDefaultByCustomerIdAsync(int customerId);
    }
}
