using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contracts.DTOs;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    /// <summary>
    /// API Controller for Bank management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BanksController : BaseApiController
    {
        private readonly IBankService _bankService;

        public BanksController(
            IBankService bankService,
            ILogger<BanksController> logger) : base(logger)
        {
            _bankService = bankService ?? throw new ArgumentNullException(nameof(bankService));
        }

        /// <summary>
        /// Get all banks
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteActionAsync(async () =>
            {
                var banks = await _bankService.GetAllBanksAsync();
                return Success(banks);
            }, "Get All Banks");
        }

        /// <summary>
        /// Get all active banks
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            return await ExecuteActionAsync(async () =>
            {
                var banks = await _bankService.GetActiveBanksAsync();
                return Success(banks);
            }, "Get Active Banks");
        }

        /// <summary>
        /// Get bank by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                var bank = await _bankService.GetByIdAsync(id);
                if (bank == null)
                    return NotFoundResponse($"Bank with ID {id} not found");

                return Success(bank);
            }, "Get Bank By Id");
        }

        /// <summary>
        /// Get bank by code
        /// </summary>
        [HttpGet("code/{bankCode}")]
        public async Task<IActionResult> GetByCode(string bankCode)
        {
            return await ExecuteActionAsync(async () =>
            {
                var bank = await _bankService.GetByCodeAsync(bankCode);
                if (bank == null)
                    return NotFoundResponse($"Bank with code '{bankCode}' not found");

                return Success(bank);
            }, "Get Bank By Code");
        }

        /// <summary>
        /// Create a new bank
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBankDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var bank = await _bankService.CreateAsync(dto);
                return Success(bank, "Bank created successfully");
            }, "Create Bank");
        }

        /// <summary>
        /// Update an existing bank
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBankDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var bank = await _bankService.UpdateAsync(id, dto);
                return Success(bank, "Bank updated successfully");
            }, "Update Bank");
        }

        /// <summary>
        /// Delete a bank
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteActionAsync(async () =>
            {
                await _bankService.DeleteAsync(id);
                return Success(new { id }, "Bank deleted successfully");
            }, "Delete Bank");
        }
    }
}
