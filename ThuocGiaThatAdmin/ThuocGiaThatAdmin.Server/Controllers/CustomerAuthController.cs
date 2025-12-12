using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contract.Requests;
using ThuocGiaThatAdmin.Server.Models;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    [Route("api/customer/auth")]
    [ApiController]
    public class CustomerAuthController : BaseApiController
    {
        private readonly ICustomerAuthService _customerAuthService;
        private readonly IZaloService _zaloService;

        public CustomerAuthController(
            ICustomerAuthService customerAuthService,
            ILogger<CustomerAuthController> logger, IZaloService zaloService) : base(logger)
        {
            _customerAuthService = customerAuthService;
            _zaloService = zaloService;
        }

        /// <summary>
        /// POST: api/customer/auth/register
        /// Register a new customer with basic information
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CustomerRegisterDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var verifyOtp = await _zaloService.VerifyOtpAsync(dto.PhoneNumber, dto.Otp);
                if (!verifyOtp)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Detail = "Mã otp không chính xác",
                        Errors = new Dictionary<string, string>
                        {
                            ["otp"] = "Mã OTP không chính xác"
                        }
                    });
                }

                var (success, message, customer) = await _customerAuthService.RegisterAsync(dto);

                if (!success)
                {
                    return BadRequestResponse(message);
                }

                var response = new
                {
                    accessToken = customer.Token,
                    tokenType = "Bearer",
                    expiresAt = customer.ExpiresAt,
                    customer = new
                    {
                        id = customer!.Id,
                        fullName = customer.FullName,
                        email = customer.Email,
                        businessTypeId = customer.BusinessTypeId,
                        businessTypeName = customer.BusinessTypeName,
                        phoneNumber = customer.PhoneNumber
                    }
                };

                return Created($"/api/customer/auth/profile", response);
            });
        }

        /// <summary>
        /// POST: api/customer/auth/login
        /// Customer login - returns JWT token
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] CustomerLoginDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var (success, message, token, expiresAt, customer) = await _customerAuthService.LoginAsync(dto);

                if (!success)
                {
                    return UnauthorizedResponse(message);
                }

                var response = new
                {
                    accessToken = token,
                    tokenType = "Bearer",
                    expiresAt = expiresAt,
                    customer = new
                    {
                        id = customer!.Id,
                        fullName = customer.FullName,
                        email = customer.Email,
                        hasBusinessInfo = customer.BusinessTypeId.HasValue,
                        businessTypeId = customer.BusinessTypeId,
                        businessTypeName = customer.BusinessType?.Name
                    }
                };

                return Success(response, "Login successful");
            });
        }

        /// <summary>
        /// POST: api/customer/auth/logout
        /// Customer logout
        /// </summary>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return ExecuteAction(() =>
            {
                // Since we are using stateless JWT, we just return success.
                // The client should discard the token.
                return Success("Logout successful");
            });
        }

        /// <summary>
        /// GET: api/customer/auth/profile
        /// Get current customer profile (requires authentication)
        /// </summary>
        [Authorize(Roles = "Customer")]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            return await ExecuteActionAsync(async () =>
            {
                var customerId = int.Parse(User.FindFirst("customer_id")?.Value ?? "0");
                var customer = await _customerAuthService.GetCustomerByIdAsync(customerId);

                if (customer == null)
                {
                    return NotFoundResponse("Customer not found");
                }

                var profile = new CustomerProfileDto
                {
                    Id = customer.Id,
                    FullName = customer.FullName,
                    Email = customer.Email,
                    PhoneNumber = customer.PhoneNumber,
                    BusinessTypeId = customer.BusinessTypeId,
                    BusinessTypeName = customer.BusinessType?.Name,
                    HasBusinessInfo = customer.BusinessTypeId.HasValue,
                    HasPaymentAccounts = customer.PaymentAccounts.Any(),
                    CreatedDate = customer.CreatedDate
                };

                return Success(profile);
            });
        }

        /// <summary>
        /// PUT: api/customer/auth/profile
        /// Update customer basic profile (requires authentication)
        /// </summary>
        [Authorize(Roles = "Customer")]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateCustomerProfileDto dto)
        {
            return await ExecuteActionAsync(async () =>
            {
                var customerId = int.Parse(User.FindFirst("customer_id")?.Value ?? "0");
                var success = await _customerAuthService.UpdateProfileAsync(customerId, dto);

                if (!success)
                {
                    return NotFoundResponse("Customer not found");
                }

                return Success<object>(null, "Profile updated successfully");
            });
        }

        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtpAsync(ZaloVerifyOtpRequest request)
        {
            return Ok();
        }

        [HttpPost("SendOtp")]
        public async Task<IActionResult> SendOtpAsync([FromBody] string phone)
        {
            await _zaloService.SendOtpMessageAsync(phone);
            return Ok();
        }

        [HttpPost("VerifyProfile")]
        public async Task<IActionResult> VerifyPhoneAndEmail(string phone, string email)
        {
            (bool success, Dictionary<string, string> errors) = await _customerAuthService.VerifyRegister(phone, email);
            if(success)
                return Ok();
            return BadRequest(new ApiErrorResponse { Detail = "", Errors = errors });
        }
    }
}
