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
        private readonly FileUploadService _fileUploadService;

        public CustomerService(ICustomerRepository customerRepository, TrueMecContext context, FileUploadService fileUploadService)
        {
            _customerRepository = customerRepository;
            _context = context;
            _fileUploadService = fileUploadService;
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
            var existingDocs = await _context.CustomerDocuments.Where(x => customerIds.Contains(x.CustomerId) && !x.IsDeleted).ToListAsync();

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
                IssuePlace = x.ProvinceId,
                FileName = x.UploadedFile.OriginalFileName
            }).ToListAsync();
        }

        /// <summary>
        /// Get all documents for a customer
        /// </summary>
        public async Task<IList<CustomerDocumentDto>> GetCustomerDocumentsAsync(int customerId)
        {
            var documents = await _context.CustomerDocuments
                .Include(x => x.UploadedFile)
                .Include(x => x.VerifiedByUser)
                .Where(x => x.CustomerId == customerId && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            return documents.Select(doc => new CustomerDocumentDto
            {
                Id = doc.Id,
                CustomerId = doc.CustomerId,
                DocumentType = doc.DocumentType,
                DocumentTypeName = doc.DocumentType.ToString(),
                UploadedFileId = doc.UploadedFileId,
                FileName = doc.UploadedFile.OriginalFileName,
                FileUrl = doc.UploadedFile.FileUrl ?? string.Empty,
                FileSize = doc.UploadedFile.FileSize,
                ContentType = doc.UploadedFile.ContentType,
                DocumentNumber = doc.DocumentNumber,
                IssueDate = doc.IssueDate,
                ExpiryDate = doc.ExpiryDate,
                IssuingAuthority = doc.IssuingAuthority,
                Notes = doc.Notes,
                ProvinceId = doc.ProvinceId,
                IsVerified = doc.IsVerified,
                VerifiedByUserId = doc.VerifiedByUserId,
                VerifiedByUserName = doc.VerifiedByUser?.UserName,
                VerifiedDate = doc.VerifiedDate,
                RejectionReason = doc.RejectionReason,
                IsRequired = doc.IsRequired,
                CreatedDate = doc.CreatedDate
            }).ToList();
        }

        /// <summary>
        /// Upload a new document for a customer
        /// </summary>
        public async Task<(bool Success, string Message, CustomerDocumentDto? Document)> UploadCustomerDocumentAsync(
            int customerId,
            UploadCustomerDocumentDto dto,
            string? uploadedByUserId = null)
        {
            // Verify customer exists
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                return (false, "Customer not found", null);
            }

            try
            {
                // Upload file using FileUploadService
                var uploadedFile = await _fileUploadService.UploadFileAsync(
                    dto.File,
                    UploadSource.Customer,
                    relatedEntityId: customerId,
                    description: $"Customer document: {dto.DocumentType}",
                    uploadedByUserId: uploadedByUserId
                );

                // Create CustomerDocument entity
                var customerDocument = new CustomerDocument
                {
                    CustomerId = customerId,
                    DocumentType = dto.DocumentType,
                    UploadedFileId = uploadedFile.Id,
                    DocumentNumber = dto.DocumentNumber,
                    IssueDate = dto.IssueDate,
                    ExpiryDate = dto.ExpiryDate,
                    IssuingAuthority = dto.IssuingAuthority,
                    Notes = dto.Notes,
                    IsRequired = dto.IsRequired,
                    IsVerified = null, // Pending verification
                    IsDeleted = false,
                    CreatedDate = DateTime.UtcNow
                };

                await _context.CustomerDocuments.AddAsync(customerDocument);
                await _context.SaveChangesAsync();

                // Reload with includes to get full data
                var createdDocument = await _context.CustomerDocuments
                    .Include(x => x.UploadedFile)
                    .Include(x => x.VerifiedByUser)
                    .FirstOrDefaultAsync(x => x.Id == customerDocument.Id);

                if (createdDocument == null)
                {
                    return (false, "Failed to create document", null);
                }

                var documentDto = new CustomerDocumentDto
                {
                    Id = createdDocument.Id,
                    CustomerId = createdDocument.CustomerId,
                    DocumentType = createdDocument.DocumentType,
                    DocumentTypeName = createdDocument.DocumentType.ToString(),
                    UploadedFileId = createdDocument.UploadedFileId,
                    FileName = createdDocument.UploadedFile.OriginalFileName,
                    FileUrl = createdDocument.UploadedFile.FileUrl ?? string.Empty,
                    FileSize = createdDocument.UploadedFile.FileSize,
                    ContentType = createdDocument.UploadedFile.ContentType,
                    DocumentNumber = createdDocument.DocumentNumber,
                    IssueDate = createdDocument.IssueDate,
                    ExpiryDate = createdDocument.ExpiryDate,
                    IssuingAuthority = createdDocument.IssuingAuthority,
                    Notes = createdDocument.Notes,
                    ProvinceId = createdDocument.ProvinceId,
                    IsVerified = createdDocument.IsVerified,
                    VerifiedByUserId = createdDocument.VerifiedByUserId,
                    VerifiedByUserName = createdDocument.VerifiedByUser?.UserName,
                    VerifiedDate = createdDocument.VerifiedDate,
                    RejectionReason = createdDocument.RejectionReason,
                    IsRequired = createdDocument.IsRequired,
                    CreatedDate = createdDocument.CreatedDate
                };

                return (true, "Document uploaded successfully", documentDto);
            }
            catch (ArgumentException ex)
            {
                // File validation errors
                return (false, ex.Message, null);
            }
            catch (Exception ex)
            {
                // Other errors
                return (false, $"Failed to upload document: {ex.Message}", null);
            }
        }

        /// <summary>
        /// Verify or reject a customer document
        /// </summary>
        public async Task<(bool Success, string Message, CustomerDocumentDto? Document)> VerifyCustomerDocumentAsync(
            int documentId,
            VerifyDocumentDto dto,
            string? verifiedByUserId = null)
        {
            // Validate: If rejected (IsApproved = false), RejectionReason is required
            if (!dto.IsApproved && string.IsNullOrWhiteSpace(dto.RejectionReason))
            {
                return (false, "Rejection reason is required when rejecting a document", null);
            }

            // Get the document
            var document = await _context.CustomerDocuments
                .Include(x => x.UploadedFile)
                .Include(x => x.VerifiedByUser)
                .FirstOrDefaultAsync(x => x.Id == documentId && !x.IsDeleted);

            if (document == null)
            {
                return (false, "Document not found", null);
            }

            // Update verification status
            document.IsVerified = dto.IsApproved;
            document.VerifiedByUserId = verifiedByUserId;
            document.VerifiedDate = DateTime.UtcNow;
            document.RejectionReason = dto.IsApproved ? null : dto.RejectionReason;

            // Update notes if provided
            if (!string.IsNullOrWhiteSpace(dto.Notes))
            {
                document.Notes = dto.Notes;
            }

            document.UpdatedDate = DateTime.UtcNow;

            _context.CustomerDocuments.Update(document);
            await _context.SaveChangesAsync();

            // Reload to get updated data
            var updatedDocument = await _context.CustomerDocuments
                .Include(x => x.UploadedFile)
                .Include(x => x.VerifiedByUser)
                .FirstOrDefaultAsync(x => x.Id == documentId);

            if (updatedDocument == null)
            {
                return (false, "Failed to verify document", null);
            }

            var documentDto = new CustomerDocumentDto
            {
                Id = updatedDocument.Id,
                CustomerId = updatedDocument.CustomerId,
                DocumentType = updatedDocument.DocumentType,
                DocumentTypeName = updatedDocument.DocumentType.ToString(),
                UploadedFileId = updatedDocument.UploadedFileId,
                FileName = updatedDocument.UploadedFile.OriginalFileName,
                FileUrl = updatedDocument.UploadedFile.FileUrl ?? string.Empty,
                FileSize = updatedDocument.UploadedFile.FileSize,
                ContentType = updatedDocument.UploadedFile.ContentType,
                DocumentNumber = updatedDocument.DocumentNumber,
                IssueDate = updatedDocument.IssueDate,
                ExpiryDate = updatedDocument.ExpiryDate,
                IssuingAuthority = updatedDocument.IssuingAuthority,
                Notes = updatedDocument.Notes,
                ProvinceId = updatedDocument.ProvinceId,
                IsVerified = updatedDocument.IsVerified,
                VerifiedByUserId = updatedDocument.VerifiedByUserId,
                VerifiedByUserName = updatedDocument.VerifiedByUser?.UserName,
                VerifiedDate = updatedDocument.VerifiedDate,
                RejectionReason = updatedDocument.RejectionReason,
                IsRequired = updatedDocument.IsRequired,
                CreatedDate = updatedDocument.CreatedDate
            };

            var message = dto.IsApproved
                ? "Document verified successfully"
                : "Document rejected successfully";

            return (true, message, documentDto);
        }

        /// <summary>
        /// Verify or reject a customer based on their documents
        /// </summary>
        public async Task<(bool Success, string Message, CustomerStatusDto? CustomerStatus)> VerifyCustomerAsync(
            int customerId,
            VerifyCustomerDto dto,
            string? verifiedByUserId = null)
        {
            // Validate: If rejected (IsApproved = false), RejectionReason is required
            if (!dto.IsApproved && string.IsNullOrWhiteSpace(dto.RejectionReason))
            {
                return (false, "Rejection reason is required when rejecting a customer", null);
            }

            // Get the customer
            var customer = await _context.Customers
                .Include(x => x.ApprovedByUser)
                .Include(x => x.Documents)
                .FirstOrDefaultAsync(x => x.Id == customerId);

            if (customer == null)
            {
                return (false, "Customer not found", null);
            }

            // If approving, validate that at least 1 document is verified
            if (dto.IsApproved)
            {
                var verifiedDocumentsCount = await _context.CustomerDocuments
                    .Where(x => x.CustomerId == customerId && x.IsVerified == true && !x.IsDeleted)
                    .CountAsync();

                if (verifiedDocumentsCount < 1)
                {
                    return (false, "Customer must have at least one verified document before approval", null);
                }
            }

            // Store old status for verification record
            var oldStatus = customer.Status;

            // Determine new status
            var newStatus = dto.IsApproved ? CustomerStatus.Approved : CustomerStatus.Suspended;

            // Check if this is the initial approval
            var isInitialApproval = dto.IsApproved && oldStatus == CustomerStatus.PendingApproval;

            // Update customer status
            customer.Status = newStatus;

            if (dto.IsApproved)
            {
                customer.ApprovedDate = DateTime.UtcNow;
                customer.ApprovedByUserId = verifiedByUserId;
            }

            customer.UpdatedDate = DateTime.UtcNow;

            // Create CustomerVerification record
            var verification = new CustomerVerification
            {
                CustomerId = customerId,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                ProcessedByUserId = verifiedByUserId,
                ProcessedDate = DateTime.UtcNow,
                Notes = dto.Notes,
                RejectionReason = dto.IsApproved ? null : dto.RejectionReason,
                Rating = dto.Rating,
                IsInitialApproval = isInitialApproval,
                CreatedDate = DateTime.UtcNow
            };

            _context.Customers.Update(customer);
            await _context.CustomerVerifications.AddAsync(verification);
            await _context.SaveChangesAsync();

            // Get document counts
            var verifiedDocsCount = await _context.CustomerDocuments
                .Where(x => x.CustomerId == customerId && x.IsVerified == true && !x.IsDeleted)
                .CountAsync();

            var totalDocsCount = await _context.CustomerDocuments
                .Where(x => x.CustomerId == customerId && !x.IsDeleted)
                .CountAsync();

            // Reload customer to get updated data
            var updatedCustomer = await _context.Customers
                .Include(x => x.ApprovedByUser)
                .FirstOrDefaultAsync(x => x.Id == customerId);

            if (updatedCustomer == null)
            {
                return (false, "Failed to verify customer", null);
            }

            // Map to DTO
            var customerStatusDto = new CustomerStatusDto
            {
                Id = updatedCustomer.Id,
                FullName = updatedCustomer.FullName,
                Email = updatedCustomer.Email,
                PhoneNumber = updatedCustomer.PhoneNumber,
                Status = updatedCustomer.Status,
                StatusName = updatedCustomer.Status.ToString(),
                ApprovedDate = updatedCustomer.ApprovedDate,
                ApprovedByUserId = updatedCustomer.ApprovedByUserId,
                ApprovedByUserName = updatedCustomer.ApprovedByUser?.UserName,
                VerifiedDocumentsCount = verifiedDocsCount,
                TotalDocumentsCount = totalDocsCount,
                CreatedDate = updatedCustomer.CreatedDate
            };

            var message = dto.IsApproved
                ? "Customer approved successfully"
                : "Customer rejected successfully";

            return (true, message, customerStatusDto);
        }

        // ========== Sales Hierarchy Methods Implementation ==========

        public async Task<IEnumerable<CustomerResponseDto>> GetCustomersBySaleUserAsync(string saleUserId)
        {
            if (string.IsNullOrWhiteSpace(saleUserId))
                throw new ArgumentNullException(nameof(saleUserId));

            var customers = await _context.Customers
                .Include(c => c.BusinessType)
                .Include(c => c.Addresses.Where(a => a.IsDefault))
                    .ThenInclude(a => a.Ward)
                .Include(c => c.Addresses.Where(a => a.IsDefault))
                    .ThenInclude(a => a.Province)
                .Where(c => c.SaleUserId == saleUserId && c.IsActive)
                .ToListAsync();

            return customers.Select(MapToResponseDto).ToList();
        }

        public async Task<IEnumerable<CustomerResponseDto>> GetCustomersBySalesTeamAsync(string managerId)
        {
            if (string.IsNullOrWhiteSpace(managerId))
                throw new ArgumentNullException(nameof(managerId));

            // Get all sale members under this manager
            var salesTeamMemberIds = await _context.Users
                .Where(u => u.ManagerId == managerId && u.IsActive)
                .Select(u => u.Id)
                .ToListAsync();

            // Get customers assigned to any member of the sales team
            var customers = await _context.Customers
                .Include(c => c.BusinessType)
                .Include(c => c.Addresses.Where(a => a.IsDefault))
                    .ThenInclude(a => a.Ward)
                .Include(c => c.Addresses.Where(a => a.IsDefault))
                    .ThenInclude(a => a.Province)
                .Where(c => c.SaleUserId != null && salesTeamMemberIds.Contains(c.SaleUserId) && c.IsActive)
                .ToListAsync();

            return customers.Select(MapToResponseDto).ToList();
        }

        public async Task<bool> AssignSaleUserAsync(int customerId, string? saleUserId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                return false;

            // Validate sale user exists if saleUserId is provided
            if (!string.IsNullOrWhiteSpace(saleUserId))
            {
                var saleUser = await _context.Users.FindAsync(saleUserId);
                if (saleUser == null || !saleUser.IsActive)
                    return false;
            }

            customer.SaleUserId = saleUserId;
            customer.UpdatedDate = DateTime.UtcNow;

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
