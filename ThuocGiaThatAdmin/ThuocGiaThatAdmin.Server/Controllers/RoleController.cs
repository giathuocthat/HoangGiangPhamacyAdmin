using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using ThuocGiaThatAdmin.Domain.Constants;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = _roleManager.Roles.Where(x => x.Name != AdminPermission.Role);
            return Ok(roles);
        }

        [HttpGet("{roleId}/detail")]
        public async Task<IActionResult> GetDetail(string roleId)
        {
            var role = _roleManager.Roles.FirstOrDefault(x => x.Id == roleId);
            return Ok(role);
        }

        [HttpGet("{userId}/roles")]
        public async Task<IActionResult> GetRoleByUserId(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }
    }
}
