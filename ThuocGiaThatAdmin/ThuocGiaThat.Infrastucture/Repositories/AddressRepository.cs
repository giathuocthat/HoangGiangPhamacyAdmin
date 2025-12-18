using Microsoft.EntityFrameworkCore;
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
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        public AddressRepository(TrueMecContext context) : base(context)
        {
        }

        public new async Task<AddressDetailResponse?> GetByIdAsync(int id)
        {
            return await _context.Set<Address>()
                .Where(x => x.Id == id)
                .Select(x => new AddressDetailResponse
                {
                    Id = x.Id,
                    CustomerId = x.CustomerId,
                    RecipientName = x.RecipientName,
                    PhoneNumber = x.PhoneNumber,
                    AddressLine = x.AddressLine,
                    WardId = x.WardId,
                    WardName = x.Ward != null ? x.Ward.Name : null,
                    ProvinceId = x.ProvinceId,
                    ProvinceName = x.Province != null ? x.Province.Name : null,
                    IsDefault = x.IsDefault,
                    AddressType = x.AddressType,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate,
                    ProvinceType = x.Province != null ? x.Province.Type : null,
                    WardType  = x.Ward!= null ? x.Ward.Type : null
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<AddressListItemDto>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Set<Address>()
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.CreatedDate)
                .Select(x => new AddressListItemDto
                {
                    Id = x.Id,
                    CustomerId = x.CustomerId,
                    RecipientName = x.RecipientName,
                    PhoneNumber = x.PhoneNumber,
                    AddressLine = x.AddressLine,
                    WardId = x.WardId,
                    WardName = x.Ward != null ? x.Ward.Name : null,
                    ProvinceId = x.ProvinceId,
                    ProvinceName = x.Province != null ? x.Province.Name : null,
                    IsDefault = x.IsDefault,
                    AddressType = x.AddressType,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate,
                    ProvinceType = x.Province != null ? x.Province.Type : null,
                    WardType = x.Ward != null ? x.Ward.Type : null

                })
                .ToListAsync();
        }

        public async Task<AddressListItemDto?> GetDefaultByCustomerIdAsync(int customerId)
        {
            return await _context.Set<Address>()
                .Where(x => x.CustomerId == customerId && x.IsDefault)
                .Select(x => new AddressListItemDto
                {
                    Id = x.Id,
                    CustomerId = x.CustomerId,
                    RecipientName = x.RecipientName,
                    PhoneNumber = x.PhoneNumber,
                    AddressLine = x.AddressLine,
                    WardId = x.WardId,
                    WardName = x.Ward != null ? x.Ward.Name : null,
                    ProvinceId = x.ProvinceId,
                    ProvinceName = x.Province != null ? x.Province.Name : null,
                    IsDefault = x.IsDefault,
                    AddressType = x.AddressType,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate,
                    ProvinceType = x.Province != null ? x.Province.Type : null,
                    WardType = x.Ward != null ? x.Ward.Type : null
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Address> CreateAsync(Address address)
        {
            await _context.Set<Address>().AddAsync(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<Address> UpdateAsync(Address address)
        {
            _context.Set<Address>().Update(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task DeleteAsync(int id)
        {
            var address = await _context.Set<Address>().FindAsync(id);
            if (address != null)
            {
                _context.Set<Address>().Remove(address);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SetDefaultAsync(int id, int customerId)
        {
            // Bỏ default của tất cả addresses khác của customer
            var addresses = await _context.Set<Address>()
                .Where(x => x.CustomerId == customerId && x.Id != id)
                .ToListAsync();

            foreach (var addr in addresses)
            {
                addr.IsDefault = false;
            }

            // Set default cho address được chọn
            var targetAddress = await _context.Set<Address>().FindAsync(id);
            if (targetAddress != null && targetAddress.CustomerId == customerId)
            {
                targetAddress.IsDefault = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
