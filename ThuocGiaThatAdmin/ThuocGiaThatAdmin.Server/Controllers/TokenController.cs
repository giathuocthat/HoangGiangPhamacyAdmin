using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contract.Responses;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace HoangGiangPhamacyAuthentication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;

        public TokenController(ITokenService tokenService, UserManager<ApplicationUser> userManager, IUserService userService)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _userService = userService;
        }   

        // POST: api/token
        [HttpPost]
        public async Task<IActionResult> Token([FromBody] LoginDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            ApplicationUser user = null;

            var emailUser = await _userManager.FindByEmailAsync(model.Email);
            var phoneUser = await _userService.FindByPhone(model.Email);

            if (emailUser != null)
            {
                user = emailUser;
            }
            if(phoneUser != null)
            {
                user = phoneUser;
            }

            if (user == null) return Unauthorized(new { error = "invalid_credentials" });

            var valid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!valid) return Unauthorized(new { error = "invalid_credentials" });

            var (accessToken, refreshToken, expiresUtc, userId) = await _tokenService.GenerateTokenAsync(user);

            var response = new TokenResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresUtc = expiresUtc,
                TokenType = "Bearer",
                UserId = userId
            };

            return Ok(response);
        }
    }

}