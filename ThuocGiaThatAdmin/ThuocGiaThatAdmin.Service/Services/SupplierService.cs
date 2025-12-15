using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service.Services
{
    /// <summary>
    /// Service implementation for Supplier business logic
    /// </summary>
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ISupplierContactRepository _supplierContactRepository;

        public SupplierService(
            ISupplierRepository supplierRepository,
            ISupplierContactRepository supplierContactRepository)
        {
            _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
            _supplierContactRepository = supplierContactRepository ?? throw new ArgumentNullException(nameof(supplierContactRepository));
        }

        public async Task<IEnumerable<SupplierDto>> GetAllAsync()
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            return suppliers.Select(MapToDto);
        }

        public async Task<SupplierDto?> GetByIdAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            return supplier != null ? MapToDto(supplier) : null;
        }

        public async Task<SupplierDto?> GetByCodeAsync(string code)
        {
            var supplier = await _supplierRepository.GetByCodeAsync(code);
            return supplier != null ? MapToDto(supplier) : null;
        }

        public async Task<IEnumerable<SupplierDto>> GetActiveSuppliersAsync()
        {
            var suppliers = await _supplierRepository.GetActiveSuppliersAsync();
            return suppliers.Select(MapToDto);
        }

        public async Task<SupplierDto?> GetWithContactsAsync(int id)
        {
            var supplier = await _supplierRepository.GetWithContactsAsync(id);
            return supplier != null ? MapToDto(supplier) : null;
        }

        public async Task<(IEnumerable<SupplierListItemDto>, int totalCount)> GetPagedSuppliersAsync(
            int pageNumber = 1,
            int pageSize = 20,
            string? searchTerm = null,
            bool? isActive = null)
        {
            var (suppliers, totalCount) = await _supplierRepository.GetPagedSuppliersAsync(
                pageNumber, pageSize, searchTerm, isActive);

            var dtos = suppliers.Select(s => new SupplierListItemDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                Email = s.Email,
                Phone = s.Phone,
                TaxCode = s.TaxCode,
                IsActive = s.IsActive,
                Rating = s.Rating,
                TotalPurchaseOrders = s.PurchaseOrders?.Count ?? 0,
                TotalPurchaseAmount = s.PurchaseOrders?.Sum(po => po.TotalAmount) ?? 0
            });

            return (dtos, totalCount);
        }

        public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Check if code already exists
            if (await _supplierRepository.CodeExistsAsync(dto.Code))
                throw new InvalidOperationException($"Supplier with code '{dto.Code}' already exists");

            var supplier = new Supplier
            {
                Code = dto.Code,
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                WardId = dto.WardId,
                ProvinceId = dto.ProvinceId,
                TaxCode = dto.TaxCode,
                BankAccount = dto.BankAccount,
                BankName = dto.BankName,
                PaymentTerms = dto.PaymentTerms,
                CreditLimit = dto.CreditLimit,
                IsActive = true,
                Rating = dto.Rating,
                Notes = dto.Notes
            };

            await _supplierRepository.AddAsync(supplier);
            await _supplierRepository.SaveChangesAsync();

            return MapToDto(supplier);
        }

        public async Task<SupplierDto> UpdateAsync(int id, UpdateSupplierDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
                throw new InvalidOperationException($"Supplier with ID {id} not found");

            // Update properties
            supplier.Name = dto.Name;
            supplier.Email = dto.Email;
            supplier.Phone = dto.Phone;
            supplier.Address = dto.Address;
            supplier.WardId = dto.WardId;
            supplier.ProvinceId = dto.ProvinceId;
            supplier.TaxCode = dto.TaxCode;
            supplier.BankAccount = dto.BankAccount;
            supplier.BankName = dto.BankName;
            supplier.PaymentTerms = dto.PaymentTerms;
            supplier.CreditLimit = dto.CreditLimit;
            supplier.IsActive = dto.IsActive;
            supplier.Rating = dto.Rating;
            supplier.Notes = dto.Notes;

            _supplierRepository.Update(supplier);
            await _supplierRepository.SaveChangesAsync();

            return MapToDto(supplier);
        }

        public async Task DeleteAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
                throw new InvalidOperationException($"Supplier with ID {id} not found");

            _supplierRepository.Delete(supplier);
            await _supplierRepository.SaveChangesAsync();
        }

        private SupplierDto MapToDto(Supplier supplier)
        {
            return new SupplierDto
            {
                Id = supplier.Id,
                Code = supplier.Code,
                Name = supplier.Name,
                Email = supplier.Email,
                Phone = supplier.Phone,
                Address = supplier.Address,
                WardId = supplier.WardId,
                WardName = supplier.Ward?.Name,
                ProvinceId = supplier.ProvinceId,
                ProvinceName = supplier.Province?.Name,
                TaxCode = supplier.TaxCode,
                BankAccount = supplier.BankAccount,
                BankName = supplier.BankName,
                PaymentTerms = supplier.PaymentTerms,
                CreditLimit = supplier.CreditLimit,
                IsActive = supplier.IsActive,
                Rating = supplier.Rating,
                Notes = supplier.Notes,
                CreatedDate = supplier.CreatedDate,
                UpdatedDate = supplier.UpdatedDate,
                Contacts = supplier.SupplierContacts?.Select(c => new SupplierContactDto
                {
                    Id = c.Id,
                    SupplierId = c.SupplierId,
                    FullName = c.FullName,
                    Position = c.Position,
                    Department = c.Department,
                    Email = c.Email,
                    Phone = c.Phone,
                    Mobile = c.Mobile,
                    ContactType = c.ContactType,
                    IsActive = c.IsActive,
                    IsPrimary = c.IsPrimary,
                    Notes = c.Notes
                }).ToList() ?? new List<SupplierContactDto>()
            };
        }
    }

    /// <summary>
    /// Service implementation for SupplierContact business logic
    /// </summary>
    public class SupplierContactService : ISupplierContactService
    {
        private readonly ISupplierContactRepository _contactRepository;
        private readonly ISupplierRepository _supplierRepository;

        public SupplierContactService(
            ISupplierContactRepository contactRepository,
            ISupplierRepository supplierRepository)
        {
            _contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
            _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
        }

        public async Task<IEnumerable<SupplierContactDto>> GetBySupplierIdAsync(int supplierId)
        {
            var contacts = await _contactRepository.GetBySupplierIdAsync(supplierId);
            return contacts.Select(MapToDto);
        }

        public async Task<SupplierContactDto?> GetByIdAsync(int id)
        {
            var contact = await _contactRepository.GetByIdAsync(id);
            return contact != null ? MapToDto(contact) : null;
        }

        public async Task<SupplierContactDto> CreateAsync(CreateSupplierContactDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Verify supplier exists
            var supplier = await _supplierRepository.GetByIdAsync(dto.SupplierId);
            if (supplier == null)
                throw new InvalidOperationException($"Supplier with ID {dto.SupplierId} not found");

            // If this is primary contact, unset other primary contacts
            if (dto.IsPrimary)
            {
                var existingPrimary = await _contactRepository.GetPrimaryContactAsync(dto.SupplierId);
                if (existingPrimary != null)
                {
                    existingPrimary.IsPrimary = false;
                    _contactRepository.Update(existingPrimary);
                }
            }

            var contact = new SupplierContact
            {
                SupplierId = dto.SupplierId,
                FullName = dto.FullName,
                Position = dto.Position,
                Department = dto.Department,
                Email = dto.Email,
                Phone = dto.Phone,
                Mobile = dto.Mobile,
                ContactType = dto.ContactType,
                IsActive = true,
                IsPrimary = dto.IsPrimary,
                Notes = dto.Notes
            };

            await _contactRepository.AddAsync(contact);
            await _contactRepository.SaveChangesAsync();

            return MapToDto(contact);
        }

        public async Task<SupplierContactDto> UpdateAsync(int id, UpdateSupplierContactDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null)
                throw new InvalidOperationException($"Supplier contact with ID {id} not found");

            // If setting as primary, unset other primary contacts
            if (dto.IsPrimary && !contact.IsPrimary)
            {
                var existingPrimary = await _contactRepository.GetPrimaryContactAsync(contact.SupplierId);
                if (existingPrimary != null && existingPrimary.Id != id)
                {
                    existingPrimary.IsPrimary = false;
                    _contactRepository.Update(existingPrimary);
                }
            }

            contact.FullName = dto.FullName;
            contact.Position = dto.Position;
            contact.Department = dto.Department;
            contact.Email = dto.Email;
            contact.Phone = dto.Phone;
            contact.Mobile = dto.Mobile;
            contact.ContactType = dto.ContactType;
            contact.IsActive = dto.IsActive;
            contact.IsPrimary = dto.IsPrimary;
            contact.Notes = dto.Notes;

            _contactRepository.Update(contact);
            await _contactRepository.SaveChangesAsync();

            return MapToDto(contact);
        }

        public async Task DeleteAsync(int id)
        {
            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null)
                throw new InvalidOperationException($"Supplier contact with ID {id} not found");

            _contactRepository.Delete(contact);
            await _contactRepository.SaveChangesAsync();
        }

        private SupplierContactDto MapToDto(SupplierContact contact)
        {
            return new SupplierContactDto
            {
                Id = contact.Id,
                SupplierId = contact.SupplierId,
                FullName = contact.FullName,
                Position = contact.Position,
                Department = contact.Department,
                Email = contact.Email,
                Phone = contact.Phone,
                Mobile = contact.Mobile,
                ContactType = contact.ContactType,
                IsActive = contact.IsActive,
                IsPrimary = contact.IsPrimary,
                Notes = contact.Notes
            };
        }
    }
}
