using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Server.Extensions;
using ThuocGiaThatAdmin.Service.Interfaces;
using ThuocGiaThatAdmin.Service.Services;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    [Authorize(Roles = "Customer")]
    public class InvoiceController : BaseApiController
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(ILogger<InvoiceController> logger, IInvoiceService invoiceService) : base(logger)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet("default")]
        public async Task<IActionResult> GetDefaultAsync()
        {
            var customerId = User.GetCustomerId();
            var dto = await _invoiceService.GetDefault(customerId);
            return Ok(dto);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync()
        {
            var customerId = User.GetCustomerId();
            var dtos = await _invoiceService.GetInvoices(customerId);

            return Ok(dtos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] InvoiceInfoDto model)
        {
            model.CustomerId = User.GetCustomerId();
            model = await _invoiceService.CreateOrUpdate(model);

            return Ok(model);
        }
    }
}
