using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace SecretSanta.Data
{
    // from https://code-maze.com/migrations-and-seed-data-efcore/#setupinitialmigration
    public static class MigrationManager
    {
        public static IHost MigrateDatabase(this IHost host, bool reseed)
        {
            using (var scope = host.Services.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<DbContext>())
                {
                    ILogger logger = Log.Logger
                        .ForContext("SourceContext", $"SecretSanta.Data.{nameof(MigrationManager)}")
                        .ForContext("Category", "Database");
                    try
                    {
                        if (reseed || (!context.Groups.Any() && !context.Users.Any()))
                        {
                            if (reseed)
                            {
                                logger.Information("Removing old database data and reseeding");
                            }
                            else
                            {
                                logger.Information("Seeding empty database");
                            }
                            context.Database.EnsureDeleted();
                            context.Database.Migrate();

                            context.Groups.AddRange(SampleData.CreateSampleData());
                            context.SaveChanges();
                        }
                        else
                        {
                            context.Database.Migrate();
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Exception encountered during startup migration", ex);
                        throw;
                    }
                }
            }
            return host;
        }
    }
}
