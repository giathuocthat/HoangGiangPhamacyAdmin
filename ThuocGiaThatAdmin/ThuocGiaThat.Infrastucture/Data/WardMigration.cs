using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ThuocGiaThat.Infrastucture.Repositories;
using ThuocGiaThatAdmin.Domain.Entities;

namespace ThuocGiaThat.Infrastucture.Data
{
    public static class WardMigration
    {
        /// <summary>
        /// Reads SQL from a ward.sql file and executes each batch (splitting on "GO") against the DB.
        /// The path can be configured with configuration key "SeedData:WardSqlPath".
        /// Default search locations (in order):
        ///  - configuration value
        ///  - {AppBaseDirectory}/Scripts/ward.sql
        ///  - {AppBaseDirectory}/ward.sql
        /// If file not found, method exits quietly (logged).
        /// </summary>
        public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("WardMigration");
            var wardService = services.GetRequiredService<IRepository<Ward>>();
            bool hasData = await wardService.AnyAsync(c => c.Id > 0);

            try
            {
                if (hasData)
                {
                    logger.LogInformation("Ward data already exists. Skipping Ward.sql seed.");
                    return;
                }

                // Determine file path
                var configuredPath = configuration["SeedData:WardSqlPath"];
                var baseDir = AppContext.BaseDirectory;
                var candidatePaths = new List<string>();

                if (!string.IsNullOrWhiteSpace(configuredPath))
                {
                    // allow relative paths in configuration
                    var maybe = Path.IsPathRooted(configuredPath)
                        ? configuredPath
                        : Path.GetFullPath(Path.Combine(baseDir, configuredPath));
                    candidatePaths.Add(maybe);
                }

                candidatePaths.Add(Path.Combine(baseDir, "Scripts", "ward.sql"));
                candidatePaths.Add(Path.Combine(baseDir, "ward.sql"));

                string? sqlFile = candidatePaths.FirstOrDefault(File.Exists);

                if (sqlFile == null)
                {
                    logger.LogWarning("ward.sql not found in any expected location. Checked: {paths}", string.Join(", ", candidatePaths));
                    return;
                }

                logger.LogInformation("Seeding countries using SQL file: {path}", sqlFile);

                var sqlText = await File.ReadAllTextAsync(sqlFile, Encoding.UTF8);

                if (string.IsNullOrWhiteSpace(sqlText))
                {
                    logger.LogWarning("ward.sql is empty. Nothing to execute.");
                    return;
                }

                // Split on lines that contain only GO (SQL Server batch separator)
                var batches = Regex.Split(sqlText, @"^\s*GO\s*?$", RegexOptions.Multiline | RegexOptions.IgnoreCase)
                                   .Select(b => b.Trim())
                                   .Where(b => !string.IsNullOrWhiteSpace(b))
                                   .ToArray();

                if (batches.Length == 0)
                {
                    logger.LogWarning("No executable SQL batches found in ward.sql.");
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
                    logger.LogInformation("Successfully executed {count} batch(es) from ward.sql.", batches.Length);
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred when seeding ward data in database.");
                throw;
            }
        }
    }
}
