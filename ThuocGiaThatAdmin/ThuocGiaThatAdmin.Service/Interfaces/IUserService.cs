using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.Responses;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult>  CreateAsync(ApplicationUser user, string password);
        Task<IdentityResult> UpdateAsync(ApplicationUser user);
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task<IEnumerable<UserResponse>> GetAllAsync(int pageIndex, int pageSize);
        Task DeactivateUser(string userName);
    }
}
