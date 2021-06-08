using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;

namespace SecretSanta.Data.Tests
{
    public class TestableDbContext : IDisposable
    {
        // adapted from from https://docs.microsoft.com/en-us/ef/core/testing/sqlite#using-sqlite-in-memory-databases
        // and https://docs.microsoft.com/en-us/ef/core/testing/testing-sample#the-tests
        // and https://github.com/dotnet/EntityFramework.Docs/blob/main/samples/core/Miscellaneous/Testing/ItemsWebApi/ItemsWebApi/ItemsContext.cs

        private DbConnection Connection { get; }
        protected DbContextOptions<DbContext> ContextOptions { get; }
        protected string SeedStr { get; } = "TestSeed";

        public TestableDbContext()
        {
            ContextOptions = new DbContextOptionsBuilder<DbContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .Options;
            Connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        }

        public void Dispose() => Connection.Dispose();

        protected static void Init(DbContext dbContext)
        {
            // since this is an in-memory db, it should be fine to repeatedly delete and repopulate it for each test
            dbContext.Database.EnsureDeleted();
            dbContext.Database.Migrate();
        }

        protected async Task Seed(DbContext dbContext)
        {
            Init(dbContext);

            List<User> users = new();
            for (int i = 0; i < 3; i++)
            {
                User u = new()
                {
                    FirstName = Guid.NewGuid().ToString(),
                    LastName = $"{SeedStr}user"
                };
                u.Gifts.Add(new Gift
                {
                    Title = $"{SeedStr}Gift {Guid.NewGuid()}",
                    Priority = 1
                });
                users.Add(u);
            }

            Group testGroup = new Group { Name = $"{SeedStr}Group" };
            testGroup.Users.AddRange(users);

            testGroup.Assignments.Add(new Assignment { Giver = users[0], Receiver = users[1] });
            testGroup.Assignments.Add(new Assignment { Giver = users[1], Receiver = users[2] });
            testGroup.Assignments.Add(new Assignment { Giver = users[2], Receiver = users[0] });

            dbContext.Groups.Add(testGroup);
            await dbContext.SaveChangesAsync();
        }
    }
}
