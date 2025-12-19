using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    /// <summary>
    /// API Controller for CustomerPaymentAccount operations
    /// </summary>
    [ApiController]
    [Route("api/customer-payment-accounts")]
    public class CustomerPaymentAccountsController : BaseApiController
    {
        private readonly ICustomerPaymentAccountService _accountService;

        public CustomerPaymentAccountsController(
            ICustomerPaymentAccountService accountService,
            ILogger<CustomerPaymentAccountsController> logger) : base(logger)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Get all payment accounts
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteActionAsync(async () =>
            {
                var accounts = await _accountService.GetAllAsync();
                return Ok(accounts);
            });
        }

        /// <summary>
        /// Get payment account by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var account = await _accountService.GetByIdAsync(id);
                if (account == null)
                    return NotFound($"CustomerPaymentAccount with ID {id} not found");

                return Ok(account);
            });
        }

        /// <summary>
        /// Get all payment accounts for a customer
        /// </summary>
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var accounts = await _accountService.GetByCustomerIdAsync(customerId);
                return Ok(accounts);
            });
        }

        /// <summary>
        /// Get default payment account for a customer
        /// </summary>
        [HttpGet("customer/{customerId}/default")]
        public async Task<IActionResult> GetDefaultAccount(int customerId)
        {
            return await ExecuteActionAsync(async () =>
            {
                var account = await _accountService.GetDefaultAccountAsync(customerId);
                if (account == null)
                    return NotFound($"No default payment account found for customer {customerId}");

                return Ok(account);
            });
        }

        /// <summary>
        /// Create a new payment account
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerPaymentAccountDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var account = await _accountService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
            });
        }

        /// <summary>
        /// Update an existing payment account
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerPaymentAccountDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var account = await _accountService.UpdateAsync(id, dto);
                return Ok(account);
            });
        }

        /// <summary>
        /// Set a payment account as default for the customer
        /// </summary>
        [HttpPut("{id}/set-default")]
        public async Task<IActionResult> SetAsDefault(int id, [FromQuery] int customerId)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _accountService.SetDefaultAccountAsync(id, customerId);
                return Ok(new { message = "Account set as default successfully" });
            });
        }

        /// <summary>
        /// Delete a payment account
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _accountService.DeleteAsync(id);
                return Ok(new { message = "Payment account deleted successfully" });
            });
        }
    }
}
