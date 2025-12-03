using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
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
    }
}
