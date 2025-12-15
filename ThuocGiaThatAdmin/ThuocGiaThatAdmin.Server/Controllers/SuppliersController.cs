using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    /// <summary>
    /// API Controller for Supplier management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : BaseApiController
    {
        private readonly ISupplierService _supplierService;
        private readonly ISupplierContactService _contactService;

        public SuppliersController(
            ISupplierService supplierService,
            ISupplierContactService contactService,
            ILogger<SuppliersController> logger) : base(logger)
        {
            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));
            _contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
        }

        /// <summary>
        /// Get all suppliers
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteActionAsync(async () =>
            {
                var suppliers = await _supplierService.GetAllAsync();
                return Success(suppliers);
            }, "Get All Suppliers");
        }

        /// <summary>
        /// Get supplier by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var supplier = await _supplierService.GetByIdAsync(id);
                if (supplier == null)
                    return NotFoundResponse($"Supplier with ID {id} not found");

                return Success(supplier);
            }, "Get Supplier By Id");
        }

        /// <summary>
        /// Get supplier by code
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            return await ExecuteActionAsync(async () =>
            {
                var supplier = await _supplierService.GetByCodeAsync(code);
                if (supplier == null)
                    return NotFoundResponse($"Supplier with code '{code}' not found");

                return Success(supplier);
            }, "Get Supplier By Code");
        }

        /// <summary>
        /// Get supplier with contacts
        /// </summary>
        [HttpGet("{id}/with-contacts")]
        public async Task<IActionResult> GetWithContacts(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var supplier = await _supplierService.GetWithContactsAsync(id);
                if (supplier == null)
                    return NotFoundResponse($"Supplier with ID {id} not found");

                return Success(supplier);
            }, "Get Supplier With Contacts");
        }

        /// <summary>
        /// Get all active suppliers
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            return await ExecuteActionAsync(async () =>
            {
                var suppliers = await _supplierService.GetActiveSuppliersAsync();
                return Success(suppliers);
            }, "Get Active Suppliers");
        }

        /// <summary>
        /// Get paged suppliers
        /// </summary>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (suppliers, totalCount) = await _supplierService.GetPagedSuppliersAsync(
                    pageNumber, pageSize, searchTerm, isActive);

                var response = new
                {
                    Data = suppliers,
                    Pagination = new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                    }
                };

                return Success(response);
            }, "Get Paged Suppliers");
        }

        /// <summary>
        /// Create a new supplier
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupplierDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var supplier = await _supplierService.CreateAsync(dto);
                return Success(supplier, "Supplier created successfully");
            }, "Create Supplier");
        }

        /// <summary>
        /// Update an existing supplier
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSupplierDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var supplier = await _supplierService.UpdateAsync(id, dto);
                return Success(supplier, "Supplier updated successfully");
            }, "Update Supplier");
        }

        /// <summary>
        /// Delete a supplier
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _supplierService.DeleteAsync(id);
                return Success(new { id }, "Supplier deleted successfully");
            }, "Delete Supplier");
        }

        // ============ Supplier Contacts ============

        /// <summary>
        /// Get contacts by supplier ID
        /// </summary>
        [HttpGet("{supplierId}/contacts")]
        public async Task<IActionResult> GetContacts(int supplierId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var contacts = await _contactService.GetBySupplierIdAsync(supplierId);
                return Success(contacts);
            }, "Get Supplier Contacts");
        }

        /// <summary>
        /// Get contact by ID
        /// </summary>
        [HttpGet("contacts/{id}")]
        public async Task<IActionResult> GetContactById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var contact = await _contactService.GetByIdAsync(id);
                if (contact == null)
                    return NotFoundResponse($"Contact with ID {id} not found");

                return Success(contact);
            }, "Get Contact By Id");
        }

        /// <summary>
        /// Create a new supplier contact
        /// </summary>
        [HttpPost("contacts")]
        public async Task<IActionResult> CreateContact([FromBody] CreateSupplierContactDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var contact = await _contactService.CreateAsync(dto);
                return Success(contact, "Contact created successfully");
            }, "Create Supplier Contact");
        }

        /// <summary>
        /// Update an existing supplier contact
        /// </summary>
        [HttpPut("contacts/{id}")]
        public async Task<IActionResult> UpdateContact(int id, [FromBody] UpdateSupplierContactDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var contact = await _contactService.UpdateAsync(id, dto);
                return Success(contact, "Contact updated successfully");
            }, "Update Supplier Contact");
        }

        /// <summary>
        /// Delete a supplier contact
        /// </summary>
        [HttpDelete("contacts/{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _contactService.DeleteAsync(id);
                return Success(new { id }, "Contact deleted successfully");
            }, "Delete Supplier Contact");
        }
    }
}
