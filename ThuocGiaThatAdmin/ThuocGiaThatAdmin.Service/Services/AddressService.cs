using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contract.Responses;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ICustomerRepository _customerRepository;

        public AddressService(
            IAddressRepository addressRepository,
            ICustomerRepository customerRepository)
        {
            _addressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        }

        public async Task<AddressDetailResponse?> GetByIdAsync(int id)
        {
            return await _addressRepository.GetByIdAsync(id);
        }

        public async Task<List<AddressListItemDto>> GetByCustomerIdAsync(int customerId)
        {
            return await _addressRepository.GetByCustomerIdAsync(customerId);
        }

        public async Task<AddressListItemDto?> GetDefaultAddress(int customerId)
        {
            return await _addressRepository.GetDefaultByCustomerIdAsync(customerId);
        }

        public async Task<AddressDetailResponse> CreateAsync(CreateAddressDto dto)
        {
            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(dto.CustomerId);
            if (customer == null)
            {
                throw new InvalidOperationException($"Customer with ID {dto.CustomerId} not found");
            }

            // If this is set as default, unset other defaults
            if (dto.IsDefault)
            {
                await _addressRepository.SetDefaultAsync(0, dto.CustomerId); // Will unset all defaults
            }

            var address = new Address
            {
                CustomerId = dto.CustomerId,
                RecipientName = dto.RecipientName,
                PhoneNumber = dto.PhoneNumber,
                AddressLine = dto.AddressLine,
                WardId = dto.WardId,
                ProvinceId = dto.ProvinceId,
                IsDefault = dto.IsDefault,
                AddressType = dto.AddressType,
                CreatedDate = DateTime.UtcNow
            };

            var createdAddress = await _addressRepository.CreateAsync(address);

            // Return the full DTO with navigation properties
            return await _addressRepository.GetByIdAsync(createdAddress.Id)
                ?? throw new InvalidOperationException("Failed to retrieve created address");
        }

        public async Task<AddressDetailResponse> UpdateAsync(int id, UpdateAddressDto dto)
        {
            var existingAddress = await _addressRepository.GetByIdAsync(id);
            if (existingAddress == null)
            {
                throw new InvalidOperationException($"Address with ID {id} not found");
            }

            // If setting as default, unset other defaults
            if (dto.IsDefault && !existingAddress.IsDefault)
            {
                await _addressRepository.SetDefaultAsync(id, existingAddress.CustomerId);
            }

            var address = new Address
            {
                Id = id,
                CustomerId = existingAddress.CustomerId,
                RecipientName = dto.RecipientName,
                PhoneNumber = dto.PhoneNumber,
                AddressLine = dto.AddressLine,
                WardId = dto.WardId,
                ProvinceId = dto.ProvinceId,
                IsDefault = dto.IsDefault,
                AddressType = dto.AddressType,
                CreatedDate = existingAddress.CreatedDate,
                UpdatedDate = DateTime.UtcNow
            };

            //await _addressRepository.UpdateAsync(address);

            // Return the full DTO with navigation properties
            return await _addressRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Failed to retrieve updated address");
        }

        public async Task DeleteAsync(int id)
        {
            var address = await _addressRepository.GetByIdAsync(id);
            if (address == null)
            {
                throw new InvalidOperationException($"Address with ID {id} not found");
            }

            await _addressRepository.DeleteAsync(id);
        }

        public async Task SetDefaultAsync(int id, int customerId)
        {
            var address = await _addressRepository.GetByIdAsync(id);
            if (address == null)
            {
                throw new InvalidOperationException($"Address with ID {id} not found");
            }

            if (address.CustomerId != customerId)
            {
                throw new InvalidOperationException($"Address {id} does not belong to customer {customerId}");
            }

            await _addressRepository.SetDefaultAsync(id, customerId);
        }
    }
}
