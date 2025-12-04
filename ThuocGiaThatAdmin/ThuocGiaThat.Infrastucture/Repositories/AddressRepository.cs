using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Repositories
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        public AddressRepository(TrueMecContext context) : base(context)
        {
        }

        public async Task<AddressDto?> GetDefaultByCustomerIdAsync(int customerId)
        {
            return await _context.Set<Address>().Where(x => x.CustomerId == customerId && x.IsDefault).Select(x => new AddressDto
            {
                Id = x.Id,
                IsDefault = x.IsDefault,
                AddressLine = x.AddressLine,
                PhoneNumber = x.PhoneNumber,
                ProvinceId = x.ProvinceId,
                ProvinceName = x.Province.Name,
                WardId = x.WardId,
                WardName = x.Ward.Name,
                RecipientName = x.RecipientName
            }).FirstOrDefaultAsync();
        }
    }
}
