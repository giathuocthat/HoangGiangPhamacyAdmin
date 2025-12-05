using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
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
    }
}
