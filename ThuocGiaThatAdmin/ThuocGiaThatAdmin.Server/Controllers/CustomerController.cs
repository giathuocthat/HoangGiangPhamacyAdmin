using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] // Require authentication for all endpoints
    public class CustomerController : BaseApiController
    {
        private readonly ICustomerService _customerService;

        public CustomerController(
            ICustomerService customerService,
            ILogger<CustomerController> logger) : base(logger)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// POST: api/customer
        /// Create a new customer (admin only)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (success, message, customer) = await _customerService.CreateCustomerAsync(dto);

                if (!success)
                {
                    return BadRequestResponse(message);
                }

                return Created($"/api/customer/{customer!.Id}", customer);
            });
        }

        /// <summary>
        /// PUT: api/customer/{id}
        /// Update an existing customer (admin only)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (success, message, customer) = await _customerService.UpdateCustomerAsync(id, dto);

                if (!success)
                {
                    if (message == "Customer not found")
                    {
                        return NotFoundResponse(message);
                    }
                    return BadRequestResponse(message);
                }

                return Success(customer, message);
            });
        }

        /// <summary>
        /// GET: api/customer/{id}
        /// Get customer by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);

                if (customer == null)
                {
                    return NotFoundResponse("Customer not found");
                }

                return Success(customer);
            });
        }

        /// <summary>
        /// GET: api/customer
        /// Get all customers with pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCustomers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return await ExecuteActionAsync(async () =>
            {
                if (pageSize > 100)
                {
                    return BadRequestResponse("Page size cannot exceed 100");
                }

                var (customers, totalCount) = await _customerService.GetCustomersAsync(pageNumber, pageSize);

                var response = new
                {
                    Data = customers,
                    Pagination = new
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                    }
                };

                return Success(response);
            });
        }

        /// <summary>
        /// GET: api/customer/search
        /// Search customers by phone number
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchByPhone([FromQuery] string phoneNumber)
        {
            return await ExecuteActionAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(phoneNumber))
                {
                    return BadRequestResponse("Phone number is required");
                }

                var customers = await _customerService.SearchByPhoneAsync(phoneNumber);
                return Success(customers);
            });
        }

        /// <summary>
        /// GET: api/customer/{id}/documents
        /// Get all documents for a customer
        /// </summary>
        [HttpGet("{id}/documents")]
        public async Task<IActionResult> GetCustomerDocuments(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var documents = await _customerService.GetCustomerDocumentsAsync(id);
                return Success(documents);
            });
        }

        /// <summary>
        /// POST: api/customer/{id}/documents
        /// Upload a new document for a customer
        /// </summary>
        [HttpPost("{id}/documents")]
        public async Task<IActionResult> UploadCustomerDocument(int id, [FromForm] UploadCustomerDocumentDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                // Get current user ID from claims (if authentication is enabled)
                // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string? userId = null; // For now, since authentication is commented out

                var (success, message, document) = await _customerService.UploadCustomerDocumentAsync(id, dto, userId);

                if (!success)
                {
                    if (message == "Customer not found")
                    {
                        return NotFoundResponse(message);
                    }
                    return BadRequestResponse(message);
                }

                return Created($"/api/customer/{id}/documents/{document!.Id}", document);
            });
        }

        /// <summary>
        /// PUT: api/customer/documents/{documentId}/verify
        /// Verify or reject a customer document
        /// </summary>
        [HttpPut("documents/{documentId}/verify")]
        public async Task<IActionResult> VerifyCustomerDocument(int documentId, [FromBody] VerifyDocumentDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                // Get current user ID from claims (if authentication is enabled)
                // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string? userId = null; // For now, since authentication is commented out

                var (success, message, document) = await _customerService.VerifyCustomerDocumentAsync(documentId, dto, userId);

                if (!success)
                {
                    if (message == "Document not found")
                    {
                        return NotFoundResponse(message);
                    }
                    return BadRequestResponse(message);
                }

                return Success(document, message);
            });
        }

        /// <summary>
        /// PUT: api/customer/{id}/verify
        /// Verify or reject a customer based on their documents
        /// </summary>
        [HttpPut("{id}/verify")]
        public async Task<IActionResult> VerifyCustomer(int id, [FromBody] VerifyCustomerDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                // Get current user ID from claims (if authentication is enabled)
                // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string? userId = null; // For now, since authentication is commented out

                var (success, message, customerStatus) = await _customerService.VerifyCustomerAsync(id, dto, userId);

                if (!success)
                {
                    if (message == "Customer not found")
                    {
                        return NotFoundResponse(message);
                    }
                    return BadRequestResponse(message);
                }

                return Success(customerStatus, message);
            });
        }

        // ========== Sales Hierarchy Endpoints ==========

        /// <summary>
        /// GET: api/customer/by-sale-user/{saleUserId}
        /// Lấy danh sách customers được assign cho một sale user
        /// </summary>
        [HttpGet("by-sale-user/{saleUserId}")]
        public async Task<IActionResult> GetCustomersBySaleUser(string saleUserId)
        {
            return await ExecuteActionAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(saleUserId))
                {
                    return BadRequestResponse("Sale user ID is required");
                }

                var customers = await _customerService.GetCustomersBySaleUserAsync(saleUserId);
                return Success(customers);
            });
        }

        /// <summary>
        /// GET: api/customer/by-sales-team/{managerId}
        /// Lấy danh sách customers của toàn bộ sales team (cho Sale Manager)
        /// </summary>
        [HttpGet("by-sales-team/{managerId}")]
        public async Task<IActionResult> GetCustomersBySalesTeam(string managerId)
        {
            return await ExecuteActionAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(managerId))
                {
                    return BadRequestResponse("Manager ID is required");
                }

                var customers = await _customerService.GetCustomersBySalesTeamAsync(managerId);
                return Success(customers);
            });
        }

        /// <summary>
        /// POST: api/customer/{customerId}/assign-sale-user
        /// Assign customer cho sale user
        /// </summary>
        [HttpPost("{customerId}/assign-sale-user")]
        public async Task<IActionResult> AssignSaleUser(int customerId, [FromBody] AssignCustomerToSaleRequest request)
        {
            return await ExecuteActionAsync(async () =>
            {
                if (!ModelState.IsValid)
                {
                    return BadRequestResponse("Invalid request");
                }

                var success = await _customerService.AssignSaleUserAsync(customerId, request.SaleUserId);

                if (!success)
                {
                    return BadRequestResponse("Failed to assign sale user. Customer or sale user not found.");
                }

                return Success(new { message = "Sale user assigned successfully" });
            });
        }
    }
}
