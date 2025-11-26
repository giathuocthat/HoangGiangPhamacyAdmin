using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Data
{
    public class CategoryMigration
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("CategoryMigration");
            var categoryRepository = services.GetRequiredService<ICategoryRepository>();

            try
            {
                // Check if categories already exist
                if (await categoryRepository.AnyAsync(x => x.Id > 0))
                {
                    logger.LogInformation("Category data already exists. Skipping seed.");
                    return;
                }

                // Step 1: Insert all root categories (ParentId = null) first
                var rootCategories = new List<Category>
                {
                    new Category { Name = "Dược phẩm", Slug = "duoc-pham", ParentId = null, DisplayOrder = 1, IsActive = true },
                    new Category { Name = "Thực phẩm chức năng", Slug = "thuc-pham-chuc-nang", ParentId = null, DisplayOrder = 2, IsActive = true },
                    new Category { Name = "Thiết bị y tế & Vật tư tiêu hao", Slug = "thiet-bi-y-te", ParentId = null, DisplayOrder = 3, IsActive = true },
                    new Category { Name = "Chăm sóc cá nhân & Làm đẹp", Slug = "cham-soc-ca-nhan", ParentId = null, DisplayOrder = 4, IsActive = true },
                    new Category { Name = "Chăm sóc mẹ và bé", Slug = "cham-soc-me-va-be", ParentId = null, DisplayOrder = 5, IsActive = true },
                    new Category { Name = "Thảo dược và Đông y", Slug = "thao-duoc-dong-y", ParentId = null, DisplayOrder = 6, IsActive = true }
                };

                // Add root categories and save to get their IDs
                await categoryRepository.AddRangeAsync(rootCategories);
                await categoryRepository.SaveChangesAsync();

                logger.LogInformation("Root categories inserted: {count}", rootCategories.Count);

                // Step 2: Retrieve the inserted root categories to get their actual IDs
                var insertedRoots = (await categoryRepository.GetAllAsync())
                    .Where(c => c.ParentId == null)
                    .OrderBy(c => c.DisplayOrder)
                    .ToList();

                // Map slug to actual ID for reference
                var categoryMap = new Dictionary<string, int>();
                foreach (var cat in insertedRoots)
                {
                    categoryMap[cat.Slug] = cat.Id;
                }

                // Step 3: Create child categories using the actual parent IDs
                var childCategories = new List<Category>();

                // Children of Dược phẩm
                if (categoryMap.TryGetValue("duoc-pham", out var duocPhamId))
                {
                    childCategories.AddRange(new[]
                    {
                        new Category { Name = "Thuốc kháng sinh", Slug = "thuoc-khang-sinh", ParentId = duocPhamId, DisplayOrder = 1, IsActive = true },
                        new Category { Name = "Thuốc tiêu hóa", Slug = "thuoc-tieu-hoa", ParentId = duocPhamId, DisplayOrder = 2, IsActive = true },
                        new Category { Name = "Thuốc tim mạch, huyết áp", Slug = "thuoc-tim-mach-huyet-ap", ParentId = duocPhamId, DisplayOrder = 3, IsActive = true },
                        new Category { Name = "Thuốc trị tiểu đường", Slug = "thuoc-tri-tieu-duong", ParentId = duocPhamId, DisplayOrder = 4, IsActive = true },
                        new Category { Name = "Thuốc giảm đau, hạ sốt", Slug = "thuoc-giam-dau-ha-sot", ParentId = duocPhamId, DisplayOrder = 5, IsActive = true },
                        new Category { Name = "Thuốc hô hấp, hen suyễn", Slug = "thuoc-ho-hap", ParentId = duocPhamId, DisplayOrder = 6, IsActive = true },
                        new Category { Name = "Thuốc ngoài da, bôi ngoài", Slug = "thuoc-ngoai-da", ParentId = duocPhamId, DisplayOrder = 7, IsActive = true }
                    });
                }

                // Children of Thực phẩm chức năng
                if (categoryMap.TryGetValue("thuc-pham-chuc-nang", out var tpcnId))
                {
                    childCategories.AddRange(new[]
                    {
                        new Category { Name = "Vitamin & Khoáng chất", Slug = "vitamin-khoang-chat", ParentId = tpcnId, DisplayOrder = 1, IsActive = true },
                        new Category { Name = "Thực phẩm bảo vệ gan", Slug = "thuc-pham-bao-ve-gan", ParentId = tpcnId, DisplayOrder = 2, IsActive = true },
                        new Category { Name = "Thực phẩm hỗ trợ xương khớp", Slug = "ho-tro-xuong-khop", ParentId = tpcnId, DisplayOrder = 3, IsActive = true },
                        new Category { Name = "Thực phẩm tăng cường sinh lý", Slug = "tang-cuong-sinh-ly", ParentId = tpcnId, DisplayOrder = 4, IsActive = true },
                        new Category { Name = "Thực phẩm hỗ trợ tiêu hóa (Probiotic)", Slug = "probiotic", ParentId = tpcnId, DisplayOrder = 5, IsActive = true },
                        new Category { Name = "Thực phẩm hỗ trợ tim mạch", Slug = "ho-tro-tim-mach-tpcn", ParentId = tpcnId, DisplayOrder = 6, IsActive = true }
                    });
                }

                // Children of Thiết bị y tế
                if (categoryMap.TryGetValue("thiet-bi-y-te", out var ytId))
                {
                    childCategories.AddRange(new[]
                    {
                        new Category { Name = "Máy đo huyết áp & máy đo đường huyết", Slug = "may-do-huyet-ap", ParentId = ytId, DisplayOrder = 1, IsActive = true },
                        new Category { Name = "Khẩu trang & dụng cụ bảo hộ", Slug = "khau-trang-bao-ho", ParentId = ytId, DisplayOrder = 2, IsActive = true },
                        new Category { Name = "Vật tư tiêu hao (Băng gạc, kim tiêm)", Slug = "vat-tu-tieu-hao", ParentId = ytId, DisplayOrder = 3, IsActive = true },
                        new Category { Name = "Nhiệt kế & dụng cụ sơ cứu", Slug = "nhiet-ke-so-cuu", ParentId = ytId, DisplayOrder = 4, IsActive = true }
                    });
                }

                // Children of Chăm sóc cá nhân & Làm đẹp
                if (categoryMap.TryGetValue("cham-soc-ca-nhan", out var chamSocId))
                {
                    childCategories.AddRange(new[]
                    {
                        new Category { Name = "Chăm sóc da mặt", Slug = "cham-soc-da-mat", ParentId = chamSocId, DisplayOrder = 1, IsActive = true },
                        new Category { Name = "Chăm sóc tóc", Slug = "cham-soc-toc", ParentId = chamSocId, DisplayOrder = 2, IsActive = true },
                        new Category { Name = "Chăm sóc răng miệng", Slug = "cham-soc-rang-mieng", ParentId = chamSocId, DisplayOrder = 3, IsActive = true },
                        new Category { Name = "Sản phẩm làm đẹp và mỹ phẩm", Slug = "my-pham-lam-dep", ParentId = chamSocId, DisplayOrder = 4, IsActive = true }
                    });
                }

                // Children of Chăm sóc mẹ và bé
                if (categoryMap.TryGetValue("cham-soc-me-va-be", out var meBeId))
                {
                    childCategories.AddRange(new[]
                    {
                        new Category { Name = "Sữa và thực phẩm cho bé", Slug = "sua-thuc-pham-cho-be", ParentId = meBeId, DisplayOrder = 1, IsActive = true },
                        new Category { Name = "Vitamin và thực phẩm cho mẹ bầu", Slug = "vitamin-me-bau", ParentId = meBeId, DisplayOrder = 2, IsActive = true },
                        new Category { Name = "Đồ dùng vệ sinh cho bé", Slug = "do-dung-ve-sinh-be", ParentId = meBeId, DisplayOrder = 3, IsActive = true }
                    });
                }

                // Step 4: Add all child categories and save
                if (childCategories.Count > 0)
                {
                    await categoryRepository.AddRangeAsync(childCategories);
                    await categoryRepository.SaveChangesAsync();
                    logger.LogInformation("Child categories inserted: {count}", childCategories.Count);
                }

                logger.LogInformation("Category seeding completed successfully. Total: {total}", rootCategories.Count + childCategories.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred when seeding categories.");
                throw;
            }
        }
    }
}
