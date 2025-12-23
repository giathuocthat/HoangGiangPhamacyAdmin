using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThat.Infrastucture.Utils;
using ThuocGiaThatAdmin.Contract.DTOs;
using ThuocGiaThatAdmin.Contract.Responses;
using ThuocGiaThatAdmin.Domain.Constants;
using ThuocGiaThatAdmin.Domain.Entities;
using ThuocGiaThatAdmin.Service.Interfaces;

namespace ThuocGiaThatAdmin.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly DynamicFilterService _dynamicFilterService;
        private readonly TrueMecContext _context;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
            IRepository<ApplicationUser> userRepository,
            DynamicFilterService dynamicFilterService,
            TrueMecContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepository = userRepository;
            _dynamicFilterService = dynamicFilterService;
            _context = context;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<ApplicationUser?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _userManager.Users.AsNoTracking().ToListAsync();
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

            var existence = await _userManager.FindByEmailAsync(user.Email);

            if (existence != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Email already exist." });
            }


            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            // Ensure attached user is the tracked entity; UserManager.UpdateAsync handles it.
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> DeleteAsync(string id)
        {
            var user = await GetByIdAsync(id);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            return await _userManager.DeleteAsync(user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await GetByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(role)) throw new ArgumentNullException(nameof(role));

            // Create role if it doesn't exist (optional)
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var addedRole = new ApplicationRole
                {
                    Name = role,
                    NormalizedName = role.ToUpper(),
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                var createRole = await _roleManager.CreateAsync(addedRole);
                if (!createRole.Succeeded) return createRole;
            }

            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string role)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(role)) throw new ArgumentNullException(nameof(role));

            return await _userManager.RemoveFromRoleAsync(user, role);
        }

        public Task<IEnumerable<UserResponse>> GetAllAsync(int pageIndex, int pageSize)
        {
            var users = _userManager.Users.Where(x => x.UserName != "ADMIN" && x.UserName != "admin")
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = users.Select(x => new UserResponse
            {
                Id = x.Id,
                FullName = x.FullName,
                UserName = x.UserName,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                CreatedDate = x.CreatedDate,
                IsActive = x.IsActive,
                Roles = (_userManager.GetRolesAsync(x).Result).Select(r => new RoleResponse { Name = r }).ToArray()
            });

            return Task.FromResult(result);
        }

        public Task<IEnumerable<UserResponse>> GetDeactivatedUsersAsync(FilterRequest request)
        {
            var query = _userRepository.AsAsQueryable();

            var filter = _dynamicFilterService.ApplyFilters(query, request);

            var users = query
                .Where(x => x.UserName != Admin.AdminUserName && x.IsActive == false)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var result = users.Select(x => new UserResponse
            {
                Id = x.Id,
                FullName = x.FullName,
                UserName = x.UserName,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                CreatedDate = x.CreatedDate,
                IsActive = x.IsActive,
                Roles = (_userManager.GetRolesAsync(x).Result).Select(r => new RoleResponse { Name = r }).ToArray()
            });

            return Task.FromResult(result);
        }

        public async Task DeactivateUser(string userName)
        {
            var userDetail = await _userRepository.FirstOrDefaultAsync(x => x.UserName == userName);

            if (userDetail != null)
            {
                userDetail.IsActive = false;
                _userRepository.Update(userDetail);
                await _userRepository.SaveChangesAsync();
            }
        }

        public async Task<ApplicationUser> FindByPhone(string phoneNumber)
        {
            var userDetail = await _userRepository.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            return userDetail;
        }

        // ========== Sales Hierarchy Methods Implementation ==========

        public async Task<IEnumerable<SalesTeamMemberDto>> GetSalesTeamMembersAsync(string managerId)
        {
            if (string.IsNullOrWhiteSpace(managerId))
                throw new ArgumentNullException(nameof(managerId));

            var salesMembers = await _userRepository
                .AsAsQueryable()
                .Where(u => u.ManagerId == managerId && u.IsActive)
                .Include(u => u.AssignedCustomers)
                .Include(u => u.Region)
                .ToListAsync();

            return salesMembers.Select(u => new SalesTeamMemberDto
            {
                Id = u.Id,
                FullName = u.FullName ?? string.Empty,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                IsActive = u.IsActive,
                AssignedCustomerCount = u.AssignedCustomers?.Count ?? 0,
                CreatedDate = u.CreatedDate,
                RegionId = u.RegionId,
                RegionName = u.Region?.Name
            });
        }

        public async Task<bool> AssignManagerAsync(string userId, string? managerId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            var user = await _userRepository.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return false;

            // Validate manager exists if managerId is provided
            if (!string.IsNullOrWhiteSpace(managerId))
            {
                var manager = await _userRepository.FirstOrDefaultAsync(u => u.Id == managerId);
                if (manager == null)
                    return false;

                // Prevent circular assignment: check if manager is in user's team
                if (await IsInSalesTeamAsync(userId, managerId))
                    return false;
            }

            user.ManagerId = managerId;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<SalesUserDto>> GetSalesUsersAsync()
        {
            // Get Sale Managers for all regions (users with "Sale Manager" role in each region)
            var salesUsers = await (from u in _context.Users
                                      join ur in _context.UserRoles on u.Id equals ur.UserId
                                      join r in _context.Roles on ur.RoleId equals r.Id
                                      where 
                                        (r.Name == SaleManagerPermission.Role || r.Name == SaleMemberPermissions.Role)
                                        && u.IsActive
                                      select new { u.Id, u.FullName, u.Email, u.PhoneNumber, u.IsActive, u.Manager, u.ManagerId, u.Region, u.RegionId })
                                      .ToListAsync();

            return salesUsers.Select(u => new SalesUserDto
            {
                Id = u.Id,
                FullName = u.FullName ?? string.Empty,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                IsActive = u.IsActive,
                ManagerId = u.ManagerId,
                ManagerName = u.Manager?.FullName,
                RegionId = u.RegionId,
                RegionName = u.Region?.Name
            });
        }

        /// <summary>
        /// Helper method to check if a user is in another user's sales team (prevent circular assignment)
        /// </summary>
        private async Task<bool> IsInSalesTeamAsync(string userId, string potentialMemberId)
        {
            var user = await _userRepository
                .AsAsQueryable()
                .Where(u => u.Id == userId)
                .Include(u => u.SalesTeamMembers)
                .FirstOrDefaultAsync();

            if (user == null) return false;

            // Direct member check
            if (user.SalesTeamMembers.Any(m => m.Id == potentialMemberId))
                return true;

            // Recursive check for nested team members
            foreach (var member in user.SalesTeamMembers)
            {
                if (await IsInSalesTeamAsync(member.Id, potentialMemberId))
                    return true;
            }

            return false;
        }

        public async Task<IEnumerable<SalesUserDto>> GetSalesManagerUsersAsync()
        {
            // Get Sale Managers for all regions (users with "Sale Manager" role in each region)
            var salesUsers = await(from u in _context.Users
                                   join ur in _context.UserRoles on u.Id equals ur.UserId
                                   join r in _context.Roles on ur.RoleId equals r.Id
                                   where
                                     (r.Name == SaleManagerPermission.Role)
                                     && u.IsActive
                                   select new { u.Id, u.FullName, u.Email, u.PhoneNumber, u.IsActive, u.Manager, u.ManagerId, u.Region, u.RegionId })
                                      .ToListAsync();

            return salesUsers.Select(u => new SalesUserDto
            {
                Id = u.Id,
                FullName = u.FullName ?? string.Empty,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                IsActive = u.IsActive,
                ManagerId = u.ManagerId,
                ManagerName = u.Manager?.FullName,
                RegionId = u.RegionId,
                RegionName = u.Region?.Name
            });
        }
    }
}
