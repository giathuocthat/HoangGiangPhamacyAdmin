using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    [Route("api/customer/business-info")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class CustomerBusinessController : BaseApiController
    {
        private readonly ICustomerAuthService _customerAuthService;

        public CustomerBusinessController(
            ICustomerAuthService customerAuthService,
            ILogger<CustomerBusinessController> logger) : base(logger)
        {
            _customerAuthService = customerAuthService;
        }

        /// <summary>
        /// GET: api/customer/business-info
        /// Get customer business information
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetBusinessInfo()
        {
            return await ExecuteActionAsync(async () =>
            {
                var customerId = int.Parse(User.FindFirst("customer_id")?.Value ?? "0");
                var customer = await _customerAuthService.GetCustomerByIdAsync(customerId);

                if (customer == null)
                {
                    return NotFoundResponse("Customer not found");
                }

                if (!customer.BusinessTypeId.HasValue)
                {
                    return NotFoundResponse("No business information found");
                }

                var businessInfo = new BusinessInfoDto
                {
                    BusinessTypeId = customer.BusinessTypeId.Value,
                    BusinessTypeName = customer.BusinessType?.Name ?? "",
                    CompanyName = customer.CompanyName ?? "",
                    TaxCode = customer.TaxCode ?? "",
                    BusinessRegistrationNumber = customer.BusinessRegistrationNumber,
                    BusinessRegistrationDate = customer.BusinessRegistrationDate,
                    LegalRepresentative = customer.LegalRepresentative,
                    BusinessLicenseUrl = customer.BusinessLicenseUrl,
                    BusinessAddress = customer.BusinessAddress,
                    BusinessPhone = customer.BusinessPhone,
                    BusinessEmail = customer.BusinessEmail
                };

                return Success(businessInfo);
            });
        }

        /// <summary>
        /// PUT: api/customer/business-info
        /// Update or create customer business information
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateBusinessInfo([FromBody] UpdateBusinessInfoDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var customerId = int.Parse(User.FindFirst("customer_id")?.Value ?? "0");
                var success = await _customerAuthService.UpdateBusinessInfoAsync(customerId, dto);

                if (!success)
                {
                    return BadRequestResponse("Failed to update business information. Please check if the business type exists.");
                }

                return Success<object>(null, "Business information updated successfully");
            });
        }
    }
}
