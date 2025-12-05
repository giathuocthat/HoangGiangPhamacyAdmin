using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace HoangGiangPhamacyAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // POST: api/user
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                FullName = dto.FullName,
                CreatedDate = DateTime.Now,
                PhoneNumber = dto.Phone,
                IsActive = true
            };

            var result = await _userService.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            // return location to the created resource
            var response = new { id = user.Id, username = user.UserName, email = user.Email };
            return Created($"/api/user/{user.Id}", response);
        }

        // PUT: api/user/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _userService.GetByIdAsync(id);
            if (existing == null) return NotFound(new { error = "User not found." });

            // Map updatable fields
            existing.FullName = dto.FullName ?? existing.FullName;
            existing.PhoneNumber = dto.PhoneNumber;

            var result = await _userService.UpdateAsync(existing);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            return Ok(new { id = existing.Id, username = existing.UserName, email = existing.Email, fullName = existing.FullName });
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { error = "id_required" });

            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { error = "user_not_found" });

            var roles = await _userService.GetRolesAsync(user);

            var dto = new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Roles = roles?.ToArray() ?? Array.Empty<string>(),
                PhoneNumber = user.PhoneNumber,
                CreatedDate = user.CreatedDate,
                IsActive = user.IsActive
            };

            return Ok(dto);
        }

        /// <summary>
        /// GET: api/user/profile/me
        /// Returns the current authenticated user's profile information.
        /// Requires a valid Bearer token in the Authorization header.
        /// </summary>
        [Authorize]
        [HttpGet("profile/me")]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            // Get the user ID from the JWT token (Subject claim)
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { error = "User ID not found in token" });

            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
                return NotFound(new { error = "user_not_found" });

            var roles = await _userService.GetRolesAsync(user);

            var dto = new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Roles = roles?.ToArray() ?? Array.Empty<string>()
            };

            return Ok(dto);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserList([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var users = await _userService.GetAllAsync(pageNumber, pageSize);

            foreach (var item in users)
            {
                item.Role = string.Join(",", item.Roles.Select(x => x.Name));
            }

            return Ok(users);
        }

        [HttpDelete("{id}/deactivate")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var userDetail = await _userService.GetByIdAsync(id);

            if (userDetail == null)
            {
                return NotFound(new { error = "user_not_found" });
            }

            await _userService.DeactivateUser(userDetail.UserName);

            return Ok();
        }
    }
}
