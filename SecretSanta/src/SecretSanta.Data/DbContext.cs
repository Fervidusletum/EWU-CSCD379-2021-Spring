using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Extensions.Hosting;
using Serilog.Sinks.SystemConsole.Themes;
using DbContext = SecretSanta.Data.DbContext;

namespace SecretSanta.Data
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        private static string Template { get; }
            = "[{Timestamp} {Level:u4}] ({Category}: {SourceContext}) {Message:lj}{NewLine}{Exception}";

        public static ILoggerFactory DbLoggerFactory { get; }
            = LoggerFactory.Create(builder => {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Category", "Database")
                    .MinimumLevel.Information()
                    .WriteTo.Console(
                        restrictedToMinimumLevel: LogEventLevel.Warning,
                        outputTemplate: Template,
                        theme: AnsiConsoleTheme.Code)
                    .WriteTo.File("db.log",
                        //restrictedToMinimumLevel: LogEventLevel.Information,
                        outputTemplate: Template)
                    .CreateLogger();

                builder.AddSerilog(logger: Log.Logger.ForContext<DbContext>());
            });

        private static Microsoft.Extensions.Logging.ILogger Logger { get; }
            = DbContext.DbLoggerFactory.CreateLogger<DbContext>();

        /*
        public DbContext(IConfiguration config)
            : this(new DbContextOptionsBuilder<DbContext>()
                  .UseSqlite($"Data Source={config?.GetValue<string>("Config:DbName") ?? "main.db"}")
                  .Options)
        {
        }
        */

        public DbContext(DbContextOptions<DbContext> options) : base(options)
        {
            Database.Migrate();
            Log.Logger.ForContext<DbContext>().Information("Logger Created");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseLoggerFactory(DbLoggerFactory);

        public override void Dispose()
        {
            Log.CloseAndFlush();
            base.Dispose();
        }

        public override ValueTask DisposeAsync()
        {
            Log.CloseAndFlush();
            return base.DisposeAsync();
        }

        public DbSet<Group> Groups => Set<Group>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Gift> Gifts => Set<Gift>();
        public DbSet<Assignment> Assignments => Set<Assignment>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null) throw new ArgumentNullException(nameof(modelBuilder));

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Users)
                .WithMany(u => u.Groups);

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Assignments)
                .WithOne(a => a.group);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Gifts)
                .WithOne(g => g.Receiver);

            //modelBuilder.Entity<Assignment>().HasOne(u => u.Giver).WithMany();
            //modelBuilder.Entity<Assignment>().HasOne(u => u.Receiver).WithMany();

            modelBuilder.Entity<User>().HasAlternateKey(user => new { user.FirstName, user.LastName });
            modelBuilder.Entity<Group>().HasAlternateKey(group => new { group.Name });
            modelBuilder.Entity<Gift>().HasAlternateKey(gift => new { gift.Title });
        }
    }
}
