using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DbContext = SecretSanta.Data.DbContext;

namespace SecretSanta.Data
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbContext()
            : base(new DbContextOptionsBuilder<DbContext>().UseSqlite("Data Source=main.db").Options)
        {
            Database.Migrate();
        }

        public DbContext(DbContextOptions<DbContext> options) : base(options)
        {
            Database.Migrate();
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
                .WithMany(u => u.UserGroups);

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Assignments)
                .WithOne(a => a.group);

            //modelBuilder.Entity<Gift>().HasOne(g => g.Receiver).WithMany(u => u.Gifts);
            modelBuilder.Entity<User>()
                .HasMany(u => u.UserGifts)
                .WithOne(g => g.Receiver);

            //modelBuilder.Entity<Assignment>().HasOne(u => u.Giver).WithMany();
            //modelBuilder.Entity<Assignment>().HasOne(u => u.Receiver).WithMany();

            modelBuilder.Entity<User>().HasAlternateKey(user => new { user.FirstName, user.LastName });
            modelBuilder.Entity<Group>().HasAlternateKey(group => new { group.Name });
            modelBuilder.Entity<Gift>().HasAlternateKey(gift => new { gift.Title });
        }
    }
}
