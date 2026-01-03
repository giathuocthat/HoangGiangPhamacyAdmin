using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IUserService userService, RoleManager<ApplicationRole> _roleManager, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            this._roleManager = _roleManager;
            _userManager = userManager;
        }

        // POST: api/user
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            //if (dto.DepartmentId == null)
            //{
            //    throw new Exception("DepartmentId is required.");
            //}

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                CreatedDate = DateTime.Now,
                PhoneNumber = dto.Phone,
                IsActive = true,
                DepartmentId = dto.DepartmentId,
                AvatarUrl = dto.AvatarUrl
            };

            var result = await _userService.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            // Assign role if provided
            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                var roleDetail = await _roleManager.FindByIdAsync(dto.Role);

                if (roleDetail != null)
                {
                    var isInRole = await _userManager.IsInRoleAsync(user, roleDetail.Name);

                    if (!isInRole)
                    {
                        await _userManager.AddToRoleAsync(user, roleDetail.Name);
                    }
                }
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
            existing.PhoneNumber = dto.Phone;
            existing.Email = dto.Email ?? existing.Email;
            existing.DepartmentId = dto.DepartmentId ?? existing.DepartmentId;
            existing.AvatarUrl = dto.AvatarUrl ?? existing.AvatarUrl;

            var result = await _userService.UpdateAsync(existing);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            // Update role if provided
            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                var roleDetail = await _roleManager.FindByNameAsync(dto.Role);

                if (roleDetail != null)
                {
                    // Remove all existing roles
                    var currentRoles = await _userManager.GetRolesAsync(existing);
                    if (currentRoles.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(existing, currentRoles);
                    }
                    
                    // Add new role
                    await _userManager.AddToRoleAsync(existing, roleDetail.Name);
                }
            }

            return Ok(new { fullName = existing.FullName, phone = existing.PhoneNumber, departmentId = existing.DepartmentId, 
                            imageUrl = existing.AvatarUrl,email = existing.Email});
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { error = "id_required" });

            var user = await _userService.GetByIdWithDepartmentAsync(id);
            if (user == null)
                return NotFound(new { error = "user_not_found" });

            var roles = await _userService.GetRolesAsync(user);

            var dto = new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                DepartmentId = user.DepartmentId,
                DepartmentName = user.Department?.Name,
                Role = roles != null && roles.Any() ? string.Join(",", roles) : string.Empty,
                Roles = roles?.ToArray() ?? Array.Empty<string>(),
                PhoneNumber = user.PhoneNumber,
                CreatedDate = user.CreatedDate,
                IsActive = user.IsActive,
                ImageUrl = user.AvatarUrl
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
                item.Role = item.Roles != null ? string.Join(",", item.Roles.Select(x => x.Name)) : string.Empty;
            }

            return Ok(users);
        }

        [HttpDelete("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(string id)
        {
            var userDetail = await _userService.GetByIdAsync(id);

            if (userDetail == null)
            {
                return NotFound(new { error = "user_not_found" });
            }

            await _userService.DeactivateUser(userDetail.UserName);

            return Ok();
        }

        [HttpPost("deactivated")]
        public async Task<IActionResult> GetDeactivatedUsers([FromBody] FilterRequest request)
        {
            var users = await _userService.GetDeactivatedUsersAsync(request);

            foreach (var item in users)
            {
                item.Role = item.Roles != null ? string.Join(",", item.Roles.Select(x => x.Name)) : string.Empty;
            }

            return Ok(users);
        }

        // ========== Sales Hierarchy Endpoints ==========

        /// <summary>
        /// GET: api/user/{managerId}/sales-team
        /// Lấy danh sách Sale Members thuộc team của một Sale Manager
        /// </summary>
        [HttpGet("{managerId}/sales-team")]
        public async Task<IActionResult> GetSalesTeamMembers(string managerId)
        {
            if (string.IsNullOrWhiteSpace(managerId))
                return BadRequest(new { error = "Manager ID is required" });

            try
            {
                var teamMembers = await _userService.GetSalesTeamMembersAsync(managerId);
                return Ok(teamMembers);
            }
            catch (ArgumentNullException)
            {
                return BadRequest(new { error = "Invalid manager ID" });
            }
        }

        /// <summary>
        /// POST: api/user/{userId}/assign-manager
        /// Assign một manager cho user
        /// </summary>
        [HttpPost("{userId}/assign-manager")]
        public async Task<IActionResult> AssignManager(string userId, [FromBody] AssignManagerRequest request)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(new { error = "User ID is required" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var success = await _userService.AssignManagerAsync(userId, request.ManagerId);

                if (!success)
                    return BadRequest(new { error = "Failed to assign manager. User or manager not found, or circular assignment detected." });

                return Ok(new { message = "Manager assigned successfully" });
            }
            catch (ArgumentNullException)
            {
                return BadRequest(new { error = "Invalid user ID" });
            }
        }

        /// <summary>
        /// GET: api/user/sales-users
        /// Lấy danh sách tất cả Sales Users (để hiển thị trong dropdown)
        /// </summary>
        [HttpGet("sales-users")]
        public async Task<IActionResult> GetSalesUsers()
        {
            var salesUsers = await _userService.GetSalesUsersAsync();
            return Ok(salesUsers);
        }

        /// <summary>
        /// GET: api/user/sales-users
        /// Lấy danh sách tất cả Sales Users (để hiển thị trong dropdown)
        /// </summary>
        [HttpGet("sales-manager-users")]
        public async Task<IActionResult> GetSalesManagerUsers()
        {
            var salesUsers = await _userService.GetSalesManagerUsersAsync();
            return Ok(salesUsers);
        }

        /// <summary>
        /// GET: api/user/by-region/{regionId}
        /// Lấy danh sách users theo regionId
        /// </summary>
        [HttpGet("by-region/{regionId}")]
        public async Task<IActionResult> GetUsersByRegion(int regionId)
        {
            var users = await _userService.GetUsersByRegionAsync(regionId);
            return Ok(users);
        }

        /// <summary>
        /// GET: api/user/by-department/{departmentId}
        /// Lấy danh sách users theo departmentId
        /// </summary>
        [HttpGet("by-department/{departmentId}")]
        public async Task<IActionResult> GetUsersByDepartment(int departmentId)
        {
            var users = await _userService.GetUsersByDepartmentAsync(departmentId);
            return Ok(users);
        }

        [HttpGet("{userId}/permissions")]
        public async Task<IActionResult> GetUsersByRegion(string userId)
        {
            var permissions = _userService.GetPermissions(userId);
            return Ok(permissions);
        }
    }
}
