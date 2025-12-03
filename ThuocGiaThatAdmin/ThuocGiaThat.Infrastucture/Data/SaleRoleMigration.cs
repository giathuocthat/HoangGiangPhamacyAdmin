using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Data
{
    public class SaleRoleMigration
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("SaleRoleMigration");

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            var saleRole = "Sale";

            try
            {
                // Ensure role exists
                if (!await roleManager.RoleExistsAsync(saleRole))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(saleRole));

                    if (!roleResult.Succeeded)
                        logger.LogError("Failed to create role {Role}: {Errors}", saleRole, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }


                logger.LogInformation("Seeding completed. Role: {AdminUser}", saleRole);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred when seeding the database.");
                throw;
            }
        }
    }
}
