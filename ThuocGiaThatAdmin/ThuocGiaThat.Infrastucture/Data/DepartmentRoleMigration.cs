using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    public class DepartmentRoleMigration
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
                    "Giám Đốc Manager" ,
                    "Giám Đốc Member" ,
                    "Marketing Manager" ,
                    "Marketing Member" ,
                    "Sale Manager" ,
                    "Sale Member" ,
                    "Purchase Manager",
                    "Purchase Member",
                    "Accountant Manager" ,
                    "Accountant Member" ,
                    "Research and Development Manager" ,
                    "Research and Development Member",
                    "IT Manager",
                    "IT Member",
                    "HR Manager",
                    "HR Member",
                    "Law Manager",
                    "Law Member",
                    "Tender Manager",
                    "Tender Member",
                    "International Market Manager",
                    "International Market Member",
                    "Inventory Manager",
                    "Inventory Member",
                     "Logistic Manager",
                     "Logistic Member",
                };

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred when seeding role claims.");
                throw;
            }
        }
    }
}
