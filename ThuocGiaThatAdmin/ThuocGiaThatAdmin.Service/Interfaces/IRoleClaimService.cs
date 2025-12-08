using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contract.Responses;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface IRoleClaimService
    {
        Task<IEnumerable<Domain.Entities.ApplicationRoleClaim>> GetAllClaimByRoleId(string roleId);
        IAsyncEnumerable<RoleClaimResponse> UpdateRoleClaims(IEnumerable<RoleClaimDto> request);
    }
}
