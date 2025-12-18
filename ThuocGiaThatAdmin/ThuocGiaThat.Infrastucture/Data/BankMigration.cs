using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Data
{
    /// <summary>
    /// Seed data cho bảng Bank - Danh sách ngân hàng Việt Nam
    /// </summary>
    public class BankMigration
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("BankMigration");
            var context = services.GetRequiredService<TrueMecContext>();

            try
            {
                // Kiểm tra xem đã có dữ liệu chưa
                if (await context.Banks.AnyAsync())
                {
                    logger.LogInformation("Bank data already exists. Skipping seed.");
                    return;
                }

                logger.LogInformation("Seeding Bank data...");

                var banks = new[]
                {
                    // Ngân hàng Nhà nước và NHTM Nhà nước
                    new Bank
                    {
                        BankCode = "VCB",
                        BankName = "Vietcombank",
                        FullName = "Ngân hàng TMCP Ngoại thương Việt Nam",
                        IsActive = true,
                        DisplayOrder = 1,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "BIDV",
                        BankName = "BIDV",
                        FullName = "Ngân hàng TMCP Đầu tư và Phát triển Việt Nam",
                        IsActive = true,
                        DisplayOrder = 2,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "VTB",
                        BankName = "Vietinbank",
                        FullName = "Ngân hàng TMCP Công Thương Việt Nam",
                        IsActive = true,
                        DisplayOrder = 3,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "AGRI",
                        BankName = "Agribank",
                        FullName = "Ngân hàng Nông nghiệp và Phát triển Nông thôn Việt Nam",
                        IsActive = true,
                        DisplayOrder = 4,
                        CreatedDate = DateTime.UtcNow
                    },

                    // Ngân hàng TMCP tư nhân lớn
                    new Bank
                    {
                        BankCode = "TCB",
                        BankName = "Techcombank",
                        FullName = "Ngân hàng TMCP Kỹ Thương Việt Nam",
                        IsActive = true,
                        DisplayOrder = 5,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "MB",
                        BankName = "MBBank",
                        FullName = "Ngân hàng TMCP Quân Đội",
                        IsActive = true,
                        DisplayOrder = 6,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "ACB",
                        BankName = "ACB",
                        FullName = "Ngân hàng TMCP Á Châu",
                        IsActive = true,
                        DisplayOrder = 7,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "VPB",
                        BankName = "VPBank",
                        FullName = "Ngân hàng TMCP Việt Nam Thịnh Vượng",
                        IsActive = true,
                        DisplayOrder = 8,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "TPB",
                        BankName = "TPBank",
                        FullName = "Ngân hàng TMCP Tiên Phong",
                        IsActive = true,
                        DisplayOrder = 9,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "STB",
                        BankName = "Sacombank",
                        FullName = "Ngân hàng TMCP Sài Gòn Thương Tín",
                        IsActive = true,
                        DisplayOrder = 10,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "HDB",
                        BankName = "HDBank",
                        FullName = "Ngân hàng TMCP Phát triển Thành phố Hồ Chí Minh",
                        IsActive = true,
                        DisplayOrder = 11,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "VIB",
                        BankName = "VIB",
                        FullName = "Ngân hàng TMCP Quốc tế Việt Nam",
                        IsActive = true,
                        DisplayOrder = 12,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "SHB",
                        BankName = "SHB",
                        FullName = "Ngân hàng TMCP Sài Gòn - Hà Nội",
                        IsActive = true,
                        DisplayOrder = 13,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "EIB",
                        BankName = "Eximbank",
                        FullName = "Ngân hàng TMCP Xuất Nhập Khẩu Việt Nam",
                        IsActive = true,
                        DisplayOrder = 14,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "MSB",
                        BankName = "MSB",
                        FullName = "Ngân hàng TMCP Hàng Hải Việt Nam",
                        IsActive = true,
                        DisplayOrder = 15,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "OCB",
                        BankName = "OCB",
                        FullName = "Ngân hàng TMCP Phương Đông",
                        IsActive = true,
                        DisplayOrder = 16,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "NAB",
                        BankName = "NamABank",
                        FullName = "Ngân hàng TMCP Nam Á",
                        IsActive = true,
                        DisplayOrder = 17,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "VAB",
                        BankName = "VietABank",
                        FullName = "Ngân hàng TMCP Việt Á",
                        IsActive = true,
                        DisplayOrder = 18,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "NCB",
                        BankName = "NCB",
                        FullName = "Ngân hàng TMCP Quốc Dân",
                        IsActive = true,
                        DisplayOrder = 19,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "SCB",
                        BankName = "SCB",
                        FullName = "Ngân hàng TMCP Sài Gòn",
                        IsActive = true,
                        DisplayOrder = 20,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "SEA",
                        BankName = "SeABank",
                        FullName = "Ngân hàng TMCP Đông Nam Á",
                        IsActive = true,
                        DisplayOrder = 21,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "LPB",
                        BankName = "LienVietPostBank",
                        FullName = "Ngân hàng TMCP Bưu Điện Liên Việt",
                        IsActive = true,
                        DisplayOrder = 22,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "VBA",
                        BankName = "VietBank",
                        FullName = "Ngân hàng TMCP Việt Nam Thương Tín",
                        IsActive = true,
                        DisplayOrder = 23,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "PGB",
                        BankName = "PGBank",
                        FullName = "Ngân hàng TMCP Xăng dầu Petrolimex",
                        IsActive = true,
                        DisplayOrder = 24,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "BAB",
                        BankName = "BacABank",
                        FullName = "Ngân hàng TMCP Bắc Á",
                        IsActive = true,
                        DisplayOrder = 25,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "PVB",
                        BankName = "PVcomBank",
                        FullName = "Ngân hàng TMCP Đại Chúng Việt Nam",
                        IsActive = true,
                        DisplayOrder = 26,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "OJB",
                        BankName = "OceanBank",
                        FullName = "Ngân hàng TMCP Đại Dương",
                        IsActive = true,
                        DisplayOrder = 27,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "GPB",
                        BankName = "GPBank",
                        FullName = "Ngân hàng TMCP Dầu Khí Toàn Cầu",
                        IsActive = true,
                        DisplayOrder = 28,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "ABB",
                        BankName = "ABBANK",
                        FullName = "Ngân hàng TMCP An Bình",
                        IsActive = true,
                        DisplayOrder = 29,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "VAF",
                        BankName = "VietCapitalBank",
                        FullName = "Ngân hàng TMCP Bản Việt",
                        IsActive = true,
                        DisplayOrder = 30,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "KLB",
                        BankName = "KienLongBank",
                        FullName = "Ngân hàng TMCP Kiên Long",
                        IsActive = true,
                        DisplayOrder = 31,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "CBB",
                        BankName = "CBBank",
                        FullName = "Ngân hàng TMCP Xây dựng Việt Nam",
                        IsActive = true,
                        DisplayOrder = 32,
                        CreatedDate = DateTime.UtcNow
                    },

                    // Ngân hàng liên doanh và nước ngoài
                    new Bank
                    {
                        BankCode = "HSBC",
                        BankName = "HSBC",
                        FullName = "Ngân hàng TNHH Một Thành Viên HSBC Việt Nam",
                        IsActive = true,
                        DisplayOrder = 33,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "SCBVL",
                        BankName = "Standard Chartered",
                        FullName = "Ngân hàng TNHH Một Thành Viên Standard Chartered Việt Nam",
                        IsActive = true,
                        DisplayOrder = 34,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "SHBVN",
                        BankName = "Shinhan Bank",
                        FullName = "Ngân hàng TNHH Một Thành Viên Shinhan Việt Nam",
                        IsActive = true,
                        DisplayOrder = 35,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "CIMB",
                        BankName = "CIMB",
                        FullName = "Ngân hàng TNHH Một Thành Viên CIMB Việt Nam",
                        IsActive = true,
                        DisplayOrder = 36,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "WVN",
                        BankName = "Woori Bank",
                        FullName = "Ngân hàng TNHH Một Thành Viên Woori Việt Nam",
                        IsActive = true,
                        DisplayOrder = 37,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "UOB",
                        BankName = "UOB",
                        FullName = "Ngân hàng United Overseas Bank Việt Nam",
                        IsActive = true,
                        DisplayOrder = 38,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "HLB",
                        BankName = "Hong Leong Bank",
                        FullName = "Ngân hàng TNHH Một Thành Viên Hong Leong Việt Nam",
                        IsActive = true,
                        DisplayOrder = 39,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Bank
                    {
                        BankCode = "PUBLIC",
                        BankName = "Public Bank",
                        FullName = "Ngân hàng TNHH Một Thành Viên Public Việt Nam",
                        IsActive = true,
                        DisplayOrder = 40,
                        CreatedDate = DateTime.UtcNow
                    }
                };

                await context.Banks.AddRangeAsync(banks);
                await context.SaveChangesAsync();

                logger.LogInformation("Successfully seeded {Count} banks", banks.Length);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding Bank data.");
                throw;
            }
        }
    }
}
