using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThuocGiaThatAdmin.Contract.DTOs;
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
        private readonly IRoleClaimService _roleClaimService;
        public RoleClaimController(RoleManager<ApplicationRole> roleManager, IRoleClaimService _service, IRoleClaimService roleClaimService)
        {
            _roleManager = roleManager;
            this._service = _service;
            _roleClaimService = roleClaimService;
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
                x.IsActive
            });

            return Ok(result);
        }

        [HttpPut("role/{roleId}")]
        public async Task<IActionResult> UpdateRoleClaims(string roleId, [FromBody] IEnumerable<RoleClaimDto> request)
        {
            var result = _roleClaimService.UpdateRoleClaims(request);

            return Ok(result);
        }
    }
}
