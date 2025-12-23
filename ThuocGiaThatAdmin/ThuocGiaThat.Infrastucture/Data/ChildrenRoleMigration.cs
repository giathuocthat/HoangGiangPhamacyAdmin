using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Domain.Constants;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Data
{
    public static class ChildrenRoleMigration
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("RoleClaimsMigration");
            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

            try
            {
                var childRoles = new List<string>()
                {
                    SaleManagerPermission.Role,
                    SaleMemberPermissions.Role,
                    SaleManagerPermission.Role
                };

                foreach (var item in childRoles)
                {
                    if (!await roleManager.RoleExistsAsync(item))
                    {
                        var addedRole = new ApplicationRole
                        {
                            Name = item,
                            NormalizedName = item.ToUpper(),
                            CreatedDate = DateTime.Now,
                            IsActive = true
                        };

                        var roleResult = await roleManager.CreateAsync(addedRole);

                        if (!roleResult.Succeeded)
                            logger.LogError("Failed to create role {Role}: {Errors}", item, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred when seeding role claims.");
                throw;
            }
        }
    }
}
