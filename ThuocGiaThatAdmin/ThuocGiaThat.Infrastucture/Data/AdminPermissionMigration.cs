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
using ThuocGiaThatAdmin.Domain.Constants;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Data
{
    public class AdminPermissionMigration
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("RoleClaimsMigration");
            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

            try
            {
                var adminRole = AdminPermission.Role;
                var claimType = AdminPermission.ClaimType;
                var saleMemberPermission = AdminPermission.GetAllPermissions();

                // Ensure SaleManager role exists
                var role = await roleManager.FindByNameAsync(adminRole);

                if (role == null)
                {
                    logger.LogWarning("Admin role not found. RoleClaims migration skipped. Please ensure UserMigration runs before RoleClaimsMigration.");
                    return;
                }


                var existingClaims = await roleManager.GetClaimsAsync(role);
                var existingPermissions = existingClaims
                    .Where(c => c.Type == claimType)
                    .Select(c => c.Value)
                    .ToHashSet();


                int addedCount = 0;
                int skippedCount = 0;

                foreach (var permission in saleMemberPermission)
                {
                    if (existingPermissions.Contains(permission))
                    {
                        skippedCount++;
                        continue;
                    }

                    var claim = new Claim(claimType, permission);
                    var result = await roleManager.AddClaimAsync(role, claim);

                    if (result.Succeeded)
                    {
                        addedCount++;
                        logger.LogInformation("Added permission claim: {Permission}", permission);
                    }
                    else
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        logger.LogError("Failed to add permission claim {Permission}: {Errors}", permission, errors);
                    }
                }


                logger.LogInformation(
                    "RoleClaims seeding completed. Added: {AddedCount}, Skipped (already exists): {SkippedCount}, Total: {TotalCount}",
                    addedCount, skippedCount, saleMemberPermission.Count);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred when seeding role claims.");
                throw;
            }
        }
    }
}
