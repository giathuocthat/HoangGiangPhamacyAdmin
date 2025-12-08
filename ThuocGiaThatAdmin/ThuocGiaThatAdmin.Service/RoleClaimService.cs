using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contract.Responses;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service
{
    public class RoleClaimService : IRoleClaimService
    {
        private readonly IRepository<ApplicationRoleClaim> _repo;
        public RoleClaimService(IRepository<ApplicationRoleClaim> repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public async Task<IEnumerable<ApplicationRoleClaim>> GetAllClaimByRoleId(string roleId)
        {
            IEnumerable<ApplicationRoleClaim> roleClaims = await _repo.FindAsync( x=> x.RoleId == roleId);

            return roleClaims;
        }

        public async IAsyncEnumerable<RoleClaimResponse> UpdateRoleClaims(IEnumerable<RoleClaimDto> request)
        {
           var roleClaims = new List<RoleClaimDto>();

            foreach (var item in request)
            {
                var roleClaim = await _repo.GetByIdAsync(item.Id);

                if (roleClaim != null)
                {
                    roleClaim.IsActive = item.IsActive;
                    _repo.Update(roleClaim);

                    yield return  new RoleClaimResponse
                    {
                        Id = roleClaim.Id,
                        ClaimType = roleClaim.ClaimType,
                        ClaimValue = roleClaim.ClaimValue,
                        IsActive = roleClaim.IsActive
                    };
                }
            }

            await _repo.SaveChangesAsync();
        }
    }
}
