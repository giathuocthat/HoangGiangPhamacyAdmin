using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    /// <summary>
    /// API Controller for Address management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AddressesController : BaseApiController
    {
        private readonly IAddressService _addressService;

        public AddressesController(
            IAddressService addressService,
            ILogger<AddressesController> logger) : base(logger)
        {
            _addressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
        }

        /// <summary>
        /// Get address by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var address = await _addressService.GetByIdAsync(id);
                if (address == null)
                    return NotFoundResponse($"Address with ID {id} not found");

                return Success(address);
            }, "Get Address By Id");
        }

        /// <summary>
        /// Get all addresses by customer ID
        /// </summary>
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var addresses = await _addressService.GetByCustomerIdAsync(customerId);
                return Success(addresses);
            }, "Get Addresses By Customer Id");
        }

        /// <summary>
        /// Get default address by customer ID
        /// </summary>
        [HttpGet("customer/{customerId}/default")]
        public async Task<IActionResult> GetDefaultByCustomerId(int customerId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var address = await _addressService.GetDefaultAddress(customerId);
                if (address == null)
                    return NotFoundResponse($"No default address found for customer {customerId}");

                return Success(address);
            }, "Get Default Address By Customer Id");
        }

        /// <summary>
        /// Create a new address
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAddressDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var address = await _addressService.CreateAsync(dto);
                return Success(address, "Address created successfully");
            }, "Create Address");
        }

        /// <summary>
        /// Update an existing address
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAddressDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var address = await _addressService.UpdateAsync(id, dto);
                return Success(address, "Address updated successfully");
            }, "Update Address");
        }

        /// <summary>
        /// Set address as default for customer
        /// </summary>
        [HttpPut("{id}/set-default")]
        public async Task<IActionResult> SetDefault(int id, [FromQuery] int customerId)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _addressService.SetDefaultAsync(id, customerId);
                return Success(new { id, customerId }, "Address set as default successfully");
            }, "Set Default Address");
        }

        /// <summary>
        /// Delete an address
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _addressService.DeleteAsync(id);
                return Success(new { id }, "Address deleted successfully");
            }, "Delete Address");
        }
    }
}
