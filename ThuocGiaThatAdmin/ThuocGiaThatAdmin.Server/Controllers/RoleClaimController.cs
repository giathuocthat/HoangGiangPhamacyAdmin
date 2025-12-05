using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleClaimController : ControllerBase
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IRoleClaimService _service;
        public RoleClaimController(RoleManager<ApplicationRole> roleManager, IRoleClaimService _service)
        {
            _roleManager = roleManager;
            this._service = _service;
        }

        [HttpGet("role/{roleId}")]
        public async Task<IActionResult> GetAll(string roleId)
        {
            var claims = await _service.GetAllClaimByRoleId(roleId);

            var result = claims.Select(x => new
            {
                x.Id,
                x.ClaimType,
                x.ClaimValue,
            });

            return Ok(result);
        }
    }
}
