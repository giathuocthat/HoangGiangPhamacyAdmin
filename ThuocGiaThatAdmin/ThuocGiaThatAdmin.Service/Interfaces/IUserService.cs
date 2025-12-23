using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contract.Responses;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThatAdmin.Service.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        Task<IdentityResult> UpdateAsync(ApplicationUser user);
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task<IEnumerable<UserResponse>> GetAllAsync(int pageIndex, int pageSize);
        Task<IEnumerable<UserResponse>> GetDeactivatedUsersAsync(FilterRequest request);
        Task DeactivateUser(string userName);
        Task<ApplicationUser> FindByPhone(string phoneNumber);

        // ========== Sales Hierarchy Methods ==========
        /// <summary>
        /// Lấy danh sách Sale Members thuộc team của một Sale Manager
        /// </summary>
        Task<IEnumerable<SalesTeamMemberDto>> GetSalesTeamMembersAsync(string managerId);

        /// <summary>
        /// Assign một manager cho user
        /// </summary>
        Task<bool> AssignManagerAsync(string userId, string? managerId);

        /// <summary>
        /// Lấy danh sách tất cả Sales Users (để hiển thị trong dropdown)
        /// </summary>
        Task<IEnumerable<SalesUserDto>> GetSalesUsersAsync();
        Task<IEnumerable<SalesUserDto>> GetSalesManagerUsersAsync();
    }

}
