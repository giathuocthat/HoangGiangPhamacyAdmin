using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Common;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contract.Requests;
using ThuocGiaThatAdmin.Contract.Responses;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly TrueMecContext _context;

        public CustomerService(ICustomerRepository customerRepository, TrueMecContext context)
        {
            _customerRepository = customerRepository;
            _context = context;
        }

        /// <summary>
        /// Create a new customer with validation
        /// </summary>
        public async Task<(bool Success, string Message, CustomerResponseDto? Customer)> CreateCustomerAsync(CreateCustomerDto dto)
        {
            // Validate phone number uniqueness
            var phoneExists = await _customerRepository.IsPhoneNumberExistsAsync(dto.PhoneNumber);
            if (phoneExists)
            {
                return (false, "Phone number already exists", null);
            }

            // Validate email uniqueness if provided
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var emailExists = await _customerRepository.IsEmailExistsAsync(dto.Email);
                if (emailExists)
                {
                    return (false, "Email already exists", null);
                }
            }

            // Create customer entity
            var customer = new Customer
            {
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                BusinessTypeId = dto.BusinessTypeId,
                PasswordHash = string.Empty, // Password is nullable for admin-created customers
                CreatedDate = DateTime.UtcNow
            };

            // Create default address
            var address = new Address
            {
                RecipientName = dto.RecipientName ?? dto.FullName,
                PhoneNumber = dto.AddressPhoneNumber ?? dto.PhoneNumber,
                AddressLine = dto.AddressLine,
                ProvinceId = dto.ProvinceId,
                WardId = dto.WardId,
                IsDefault = true,
                CreatedDate = DateTime.UtcNow
            };

            customer.Addresses.Add(address);

            // Save to database
            await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync();

            // Reload customer with includes to get BusinessType
            var createdCustomer = await _customerRepository.GetByIdWithIncludesAsync(customer.Id);

            if (createdCustomer == null)
            {
                return (false, "Failed to create customer", null);
            }

            var response = MapToResponseDto(createdCustomer);
            return (true, "Customer created successfully", response);
        }

        /// <summary>
        /// Update existing customer with validation
        /// </summary>
        public async Task<(bool Success, string Message, CustomerResponseDto? Customer)> UpdateCustomerAsync(int id, UpdateCustomerDto dto)
        {
            // Get existing customer
            var customer = await _customerRepository.GetByIdWithIncludesAsync(id);
            if (customer == null)
            {
                return (false, "Customer not found", null);
            }

            // Validate phone number uniqueness (excluding current customer)
            var phoneExists = await _customerRepository.IsPhoneNumberExistsAsync(dto.PhoneNumber, id);
            if (phoneExists)
            {
                return (false, "Phone number already exists", null);
            }

            // Validate email uniqueness if provided (excluding current customer)
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var emailExists = await _customerRepository.IsEmailExistsAsync(dto.Email, id);
                if (emailExists)
                {
                    return (false, "Email already exists", null);
                }
            }

            // Update customer fields
            customer.FullName = dto.FullName;
            customer.PhoneNumber = dto.PhoneNumber;
            customer.Email = dto.Email;
            customer.BusinessTypeId = dto.BusinessTypeId;
            customer.UpdatedDate = DateTime.UtcNow;

            // Update or create default address
            var defaultAddress = customer.Addresses.FirstOrDefault(a => a.IsDefault);
            if (defaultAddress != null)
            {
                // Update existing default address
                defaultAddress.RecipientName = dto.RecipientName ?? dto.FullName;
                defaultAddress.PhoneNumber = dto.AddressPhoneNumber ?? dto.PhoneNumber;
                defaultAddress.AddressLine = dto.AddressLine;
                defaultAddress.ProvinceId = dto.ProvinceId;
                defaultAddress.WardId = dto.WardId;
                defaultAddress.UpdatedDate = DateTime.UtcNow;
            }
            else
            {
                // Create new default address if none exists
                var newAddress = new Address
                {
                    RecipientName = dto.RecipientName ?? dto.FullName,
                    PhoneNumber = dto.AddressPhoneNumber ?? dto.PhoneNumber,
                    AddressLine = dto.AddressLine,
                    ProvinceId = dto.ProvinceId,
                    WardId = dto.WardId,
                    IsDefault = true,
                    CreatedDate = DateTime.UtcNow
                };
                customer.Addresses.Add(newAddress);
            }

            // Save changes
            _customerRepository.Update(customer);
            await _customerRepository.SaveChangesAsync();

            // Reload customer with includes
            var updatedCustomer = await _customerRepository.GetByIdWithIncludesAsync(id);
            if (updatedCustomer == null)
            {
                return (false, "Failed to update customer", null);
            }

            var response = MapToResponseDto(updatedCustomer);
            return (true, "Customer updated successfully", response);
        }

        /// <summary>
        /// Get customer by ID
        /// </summary>
        public async Task<CustomerResponseDto?> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerRepository.GetByIdWithIncludesAsync(id);
            return customer == null ? null : MapToResponseDto(customer);
        }

        /// <summary>
        /// Get all customers with pagination
        /// </summary>
        public async Task<(IEnumerable<CustomerResponseDto> Customers, int TotalCount)> GetCustomersAsync(int pageNumber, int pageSize)
        {
            var (customers, totalCount) = await _customerRepository.GetAllWithPaginationAsync(pageNumber, pageSize);
            var customerDtos = customers.Select(MapToResponseDto).ToList();
            return (customerDtos, totalCount);
        }

        /// <summary>
        /// Search customers by phone number
        /// </summary>
        public async Task<IEnumerable<CustomerResponseDto>> SearchByPhoneAsync(string phoneNumber)
        {
            var customers = await _customerRepository.SearchByPhoneAsync(phoneNumber);
            return customers.Select(MapToResponseDto).ToList();
        }

        /// <summary>
        /// Map Customer entity to CustomerResponseDto
        /// </summary>
        private CustomerResponseDto MapToResponseDto(Customer customer)
        {
            var defaultAddress = customer.Addresses.FirstOrDefault(a => a.IsDefault);

            return new CustomerResponseDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                BusinessTypeId = customer.BusinessTypeId,
                BusinessTypeName = customer.BusinessType?.Name,
                Address = defaultAddress != null ? new AddressDto
                {
                    Id = defaultAddress.Id,
                    RecipientName = defaultAddress.RecipientName,
                    PhoneNumber = defaultAddress.PhoneNumber,
                    AddressLine = defaultAddress.AddressLine,
                    WardId = defaultAddress.WardId,
                    WardName = defaultAddress.Ward?.Name,
                    ProvinceId = defaultAddress.ProvinceId,
                    ProvinceName = defaultAddress.Province?.Name,
                    IsDefault = defaultAddress.IsDefault
                } : null,
                CreatedDate = customer.CreatedDate,
                UpdatedDate = customer.UpdatedDate
            };
        }

        public async Task UpdateLicenses(IList<CustomerDocumentDto> documents)
        {
            var customerIds = documents.Select(x => x.CustomerId).Distinct().ToList();
            var existingDocs = await _context.CustomerDocuments.Where(x =>  customerIds.Contains(x.CustomerId) && !x.IsDeleted).ToListAsync();

            var existingDict = existingDocs.ToLookup(x => new { x.CustomerId, x.DocumentType });

            var newDocuments = new List<CustomerDocument>();
            var existingDocuments = new List<CustomerDocument>();
            
            foreach (var document in documents) 
            {
                var key = new { document.CustomerId, document.DocumentType };
                var existingItem = existingDict[key].FirstOrDefault();
                if (existingItem != null)
                {
                    existingItem.UpdatedDate = DateTime.UtcNow;
                    existingItem.IsDeleted = true;
                    existingDocuments.Add(existingItem);
                }

                newDocuments.Add(new CustomerDocument
                {
                    IssueDate = document.IssueDate,
                    UploadedFileId = document.UploadedFileId,
                    CustomerId = document.CustomerId,
                    DocumentNumber = document.DocumentNumber,
                    CreatedDate = DateTime.UtcNow,
                    ProvinceId = document.ProvinceId,
                    DocumentType = document.DocumentType,
                });

            }

            if (newDocuments.Any())
            {
                await _context.CustomerDocuments.AddRangeAsync(newDocuments);
            }

            await _context.SaveChangesAsync();

        }

        public async Task<IList<CustomerLicenseResponse>> GetLicenses(int customerId)
        {
            return await _context.CustomerDocuments.Where(x => x.CustomerId == customerId && !x.IsDeleted).Select(x => new CustomerLicenseResponse
            {
                Id = x.Id,
                Number = x.DocumentNumber,
                IssueDate = x.IssueDate,
                Type = (int)x.DocumentType,
                FilePath = x.UploadedFile.FileUrl,
                IssuePlace = x.ProvinceId
            }).ToListAsync();
        }
    }
}
