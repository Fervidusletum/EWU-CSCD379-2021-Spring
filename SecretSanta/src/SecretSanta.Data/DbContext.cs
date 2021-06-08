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
        // purely for letting "dotnet ef migrations" do its thing, it gets mad without it
        public DbContext()
            : this(new DbContextOptionsBuilder<DbContext>().UseSqlite("Data Source=main.db").Options)
        {
        }

        public DbContext(DbContextOptions<DbContext> options) : base(options)
        {
            //Database.Migrate();
        }

        public DbSet<Group> Groups => Set<Group>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Gift> Gifts => Set<Gift>();
        public DbSet<Assignment> Assignments => Set<Assignment>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null) throw new ArgumentNullException(nameof(modelBuilder));

            //modelBuilder.Entity<Group>().HasIndex(g => g.Name).IsUnique();
            modelBuilder.Entity<Group>()
                .HasMany(g => g.Users)
                .WithMany(u => u.Groups);
            modelBuilder.Entity<Group>()
                .HasMany(g => g.Assignments)
                .WithOne(a => a.Group);

            //modelBuilder.Entity<User>().HasIndex(u => new { u.FirstName, u.LastName });
            modelBuilder.Entity<User>()
                .HasMany(u => u.Gifts)
                .WithOne(g => g.Receiver);

            //modelBuilder.Entity<Gift>().HasIndex(g => new { g.Title, g.ReceiverId });

            //modelBuilder.Entity<Assignment>().HasIndex(a => new { a.AssignmentGroupId, a.GiverId, a.ReceiverId });

            /*
            modelBuilder.Entity<Group>()
                .HasData(SampleData.SeedGroups());
            modelBuilder.Entity<User>()
                .HasData(SampleData.SeedUsers());
            */
        }
    }
}
