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
    /// <summary>
    /// Seeds role claims (permissions) for the Admin role.
    /// This migration should run after UserMigration to ensure the Admin role exists.
    /// </summary>
    public static class RoleClaimsMigration
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("RoleClaimsMigration");
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            try
            {
                var adminRole = "Admin";

                // Ensure Admin role exists
                var role = await roleManager.FindByNameAsync(adminRole);
                if (role == null)
                {
                    logger.LogWarning("Admin role not found. RoleClaims migration skipped. Please ensure UserMigration runs before RoleClaimsMigration.");
                    return;
                }

                // Get all permissions from the Permissions constants class
                var adminPermissions = AdminPermissions.GetAllPermissions();

                // Get existing claims for the Admin role
                var existingClaims = await roleManager.GetClaimsAsync(role);
                var existingPermissions = existingClaims
                    .Where(c => c.Type == AdminPermissions.ClaimType)
                    .Select(c => c.Value)
                    .ToHashSet();


                int addedCount = 0;
                int skippedCount = 0;

                // Add each permission as a claim if it doesn't already exist
                foreach (var permission in adminPermissions)
                {
                    if (existingPermissions.Contains(permission))
                    {
                        skippedCount++;
                        continue;
                    }

                    var claim = new Claim(AdminPermissions.ClaimType, permission);
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
                    addedCount, skippedCount, adminPermissions.Count);




                var saleRole = "Sale";

                // Ensure Admin role exists
                var saleRoleDetail = await roleManager.FindByNameAsync(saleRole);

                if (role == null)
                {
                    logger.LogWarning("Sale role not found. RoleClaims migration skipped. Please ensure UserMigration runs before RoleClaimsMigration.");
                    return;
                }

                //// Get all permissions from the Permissions constants class
                var salePermission = SalePermissions.GetAllPermissions();

                // Get existing claims for the Admin role
                var saleExistingClaims = await roleManager.GetClaimsAsync(saleRoleDetail);

                var existingSlaePermissions = saleExistingClaims
                    .Where(c => c.Type == SalePermissions.ClaimType)
                    .Select(c => c.Value)
                    .ToHashSet();

                addedCount = 0;
                skippedCount = 0;

                var salePermissions = SalePermissions.GetAllPermissions();

                // Add each permission as a claim if it doesn't already exist
                foreach (var permission in salePermissions)
                {
                    if (existingSlaePermissions.Contains(permission))
                    {
                        skippedCount++;
                        continue;
                    }

                    var claim = new Claim(SalePermissions.ClaimType, permission);
                    var result = await roleManager.AddClaimAsync(saleRoleDetail, claim);

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
                    addedCount, skippedCount, salePermissions.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred when seeding role claims.");
                throw;
            }
        }
    }
}
