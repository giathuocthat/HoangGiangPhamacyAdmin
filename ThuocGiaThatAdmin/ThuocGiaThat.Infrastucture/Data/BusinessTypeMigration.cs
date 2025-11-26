using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Interfaces;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Data
{
    public class BusinessTypeMigration
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("BusinessTypeMigration");
            var businessTypeRepository = services.GetRequiredService<IBusinessTypeRepository>();

            try
            {
                // Ensure role exists
                if (await businessTypeRepository.AnyAsync(x => x.Id > 0))
                {
                    logger.LogInformation("BusinessType data already exists. Skipping country manager seed.");
                    return;
                }

                var businessTypes = new List<BusinessType>()
                {
                    new() { Name = "Quầy thuốc" },
                    new() { Name = "Nhà thuốc" },
                    new() { Name = "Phòng khám" },
                    new() { Name = "Bệnh viện" },
                    new() { Name = "Công ty dược phẩm" },
                    new() { Name = "Nha khoa" },
                    new() { Name = "Thẩm mỹ viện" },
                    new() { Name = "Trung tâm y tế" },
                    new() { Name = "Dược sĩ" },
                };

                await businessTypeRepository.AddRangeAsync(businessTypes);
                await businessTypeRepository.SaveChangesAsync();

                logger.LogInformation("Seeding data BusinessType completed.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred when seeding the database.");
                throw;
            }
        }
    }
}
