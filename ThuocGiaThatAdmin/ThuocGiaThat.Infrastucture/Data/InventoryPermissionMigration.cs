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
    public static class InventoryPermissionMigration
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("RoleClaimsMigration");
            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

            try
            {
                var inventoryRole = "Inventory Manager";

                // Ensure Admin role exists
                var role = await roleManager.FindByNameAsync(inventoryRole);
                if (role == null)
                {
                    logger.LogWarning("Admin role not found. RoleClaims migration skipped. Please ensure UserMigration runs before RoleClaimsMigration.");
                    return;
                }

                // Get all permissions from the Permissions constants class
                var inventoryPermission = InventoryPermission.GetAllPermissions();

                // Get existing claims for the Admin role
                var existingClaims = await roleManager.GetClaimsAsync(role);
                var existingPermissions = existingClaims
                    .Where(c => c.Type == InventoryPermission.ClaimType)
                    .Select(c => c.Value)
                    .ToHashSet();


                int addedCount = 0;
                int skippedCount = 0;

                // Add each permission as a claim if it doesn't already exist
                foreach (var permission in inventoryPermission)
                {
                    if (existingPermissions.Contains(permission))
                    {
                        skippedCount++;
                        continue;
                    }

                    var claim = new Claim(InventoryPermission.ClaimType, permission);
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
                    addedCount, skippedCount, inventoryPermission.Count);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred when seeding role claims.");
                throw;
            }
        }
    }
}
