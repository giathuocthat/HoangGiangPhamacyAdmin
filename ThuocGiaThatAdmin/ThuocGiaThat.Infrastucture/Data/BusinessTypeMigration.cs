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
            var businessTypeRepository= services.GetRequiredService<IBusinessTypeRepository>();

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
                    new() { Id = 1, Name = "Quầy thuốc" },
                    new() { Id = 2, Name = "Nhà thuốc" },
                    new() { Id = 3, Name = "Phòng khám" },
                    new() { Id = 4, Name = "Bệnh viện" },
                    new() { Id = 5, Name = "Công ty dược phẩm" },
                    new() { Id = 6, Name = "Nha khoa" },
                    new() { Id = 7, Name = "Thẩm mỹ viện" },
                    new() { Id = 8, Name = "Trung tâm y tế" },
                    new() { Id = 9, Name = "Dược sĩ" },
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
