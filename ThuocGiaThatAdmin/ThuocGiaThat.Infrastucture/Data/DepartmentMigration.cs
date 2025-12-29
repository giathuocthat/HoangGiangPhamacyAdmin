using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.RegularExpressions;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Data
{
    public static class DepartmentMigration
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DepartmentMigration");
            var departmentRepository = services.GetRequiredService<IDepartmentRepository>();

            bool hasData = await departmentRepository.AsAsQueryable().AnyAsync(d => d.Id > 0);

            try
            {
                if (hasData)
                {
                    logger.LogInformation("Departments data already exists. Skipping department.sql seed.");
                    return;
                }

                // Determine file path
                var baseDir = AppContext.BaseDirectory;
                var candidatePaths = new List<string>();

        
                candidatePaths.Add(Path.Combine(baseDir, "Scripts", "department.sql"));


                string? sqlFile = candidatePaths.FirstOrDefault(File.Exists);

                if (sqlFile == null)
                {
                    logger.LogWarning("department.sql not found in any expected location. Checked: {paths}", string.Join(", ", candidatePaths));
                    return;
                }

                logger.LogInformation("Seeding departments using SQL file: {path}", sqlFile);

                var sqlText = await File.ReadAllTextAsync(sqlFile, Encoding.UTF8);

                if (string.IsNullOrWhiteSpace(sqlText))
                {
                    logger.LogWarning("province.sql is empty. Nothing to execute.");
                    return;
                }

                // Split on lines that contain only GO (SQL Server batch separator)
                var batches = Regex.Split(sqlText, @"^\s*GO\s*?$", RegexOptions.Multiline | RegexOptions.IgnoreCase)
                                   .Select(b => b.Trim())
                                   .Where(b => !string.IsNullOrWhiteSpace(b))
                                   .ToArray();

                if (batches.Length == 0)
                {
                    logger.LogWarning("No executable SQL batches found in province.sql.");
                    return;
                }

                // Get DbContext and execute batches inside a transaction
                var db = services.GetRequiredService<TrueMecContext>();

                // Use a transaction to ensure atomicity of the seed script
                await using var tx = await db.Database.BeginTransactionAsync();
                try
                {
                    foreach (var batch in batches)
                    {
                        logger.LogDebug("Executing SQL batch (length {len})", batch.Length);
                        await db.Database.ExecuteSqlRawAsync(batch);
                    }

                    await tx.CommitAsync();
                    logger.LogInformation("Successfully executed {count} batch(es) from department.sql.", batches.Length);
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred when seeding department data in database.");
                throw;
            }
        }
    }
}
