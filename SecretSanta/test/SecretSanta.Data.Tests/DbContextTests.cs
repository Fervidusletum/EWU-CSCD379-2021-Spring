using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SecretSanta.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;

namespace SecretSanta.Data.Tests
{
    [TestClass]
    public class DbContextTests : IDisposable
    {
        #region In-Memory Testing Setup

        private DbConnection Connection { get; }
        protected DbContextOptions<DbContext> ContextOptions { get; }
        protected string SeedStr { get; } = "TestSeed";

        public DbContextTests()
        {
            ContextOptions = new DbContextOptionsBuilder<DbContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .Options;
            Connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
            Seed();
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        }

        private void Seed()
        {
            using DbContext dbContext = new(ContextOptions);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.Migrate();

            Group testGroup = new Group { Name = $"{SeedStr}Group" };

            List<User> users = new();
            for (int i = 0; i < 3; i++)
            {
                User u = new()
                {
                    FirstName = Guid.NewGuid().ToString(),
                    LastName = $"{SeedStr}user"
                };
                u.UserGroups.Add(testGroup);
                u.UserGifts.Add(new Gift
                {
                    Title = $"{SeedStr}Gift {Guid.NewGuid()}",
                    Priority = 1
                });
                users.Add(u);
            }

            testGroup.Users.AddRange(users);
            testGroup.Assignments.Add(new Assignment(users[0], users[1]));
            testGroup.Assignments.Add(new Assignment(users[1], users[2]));
            testGroup.Assignments.Add(new Assignment(users[2], users[0]));

            dbContext.Groups.Add(testGroup);
            dbContext.SaveChanges();
        }

        public void Dispose() => Connection.Dispose();

        #endregion


        [TestMethod]
        public void DbContext_GetUser_IsNotNull()
        {
            using DbContext dbContext = new(ContextOptions);

            User? usr = dbContext.Users.FirstOrDefault<User>(u => u.LastName.StartsWith(SeedStr));

            Assert.IsNotNull(usr);
        }

        [TestMethod]
        async public Task DbContext_AddingUser_CountIncrementsByOne()
        {
            using DbContext dbContext = new(ContextOptions);
            string fn = Guid.NewGuid().ToString();
            string ln = nameof(DbContextTests) + nameof(DbContext_AddingUser_CountIncrementsByOne);

            async Task RemoveExistingTestUserAsync()
            {
                dbContext.Users.RemoveRange(
                    dbContext.Users.Where( user => user.LastName == ln ));
                await dbContext.SaveChangesAsync();
            }

            try
            {
                await RemoveExistingTestUserAsync();
                int expected = dbContext.Users.Count() + 1;

                dbContext.Users.Add(new User { FirstName = fn, LastName = ln });
                await dbContext.SaveChangesAsync();

                Assert.AreEqual<int>(expected, dbContext.Users.Count());
            }
            finally
            {
                await RemoveExistingTestUserAsync();
            }
        }

        [TestMethod]
        async public Task DbContext_RemovingUser_CountDecrementsByOne()
        {
            using DbContext dbContext = new(ContextOptions);
            string fn = Guid.NewGuid().ToString();
            string ln = nameof(DbContextTests) + nameof(DbContext_AddingUser_CountIncrementsByOne);

            async Task RemoveExistingTestUserAsync()
            {
                dbContext.Users.RemoveRange(
                    dbContext.Users.Where( user => user.LastName == ln ));
                await dbContext.SaveChangesAsync();
            }

            try
            {
                await RemoveExistingTestUserAsync();
                dbContext.Users.Add(new User { FirstName = fn, LastName = ln });
                await dbContext.SaveChangesAsync();
                int expected = dbContext.Users.Count() - 1;

                dbContext.Users.RemoveRange(
                    dbContext.Users.Where(u => u.FirstName == fn && u.LastName == ln ));
                await dbContext.SaveChangesAsync();

                Assert.AreEqual<int>(expected, dbContext.Users.Count());
            }
            finally
            {
                await RemoveExistingTestUserAsync();
            }
        }


        // something's weird on this test, it has missing data depending on order stuff is retreived
        [TestMethod]
        public void DbContext_GetGiftViaDbList_IsLinkedBackToUser()
        {
            using DbContext dbContext = new(ContextOptions);
            User? firstUser = dbContext.Users.FirstOrDefault<User>();
            Assert.IsNotNull(firstUser, "Could not get first user.");

            Gift? giftFromList = dbContext.Gifts.FirstOrDefault<Gift>();
            Assert.IsNotNull(giftFromList, "Could not get gift from DB list.");
            User? actual = giftFromList!.Receiver;
            Assert.IsNotNull(actual, "Could not get user from DB list gift.");

            Gift? firstGift = firstUser!.UserGifts.FirstOrDefault<Gift>();
            Assert.IsNotNull(firstGift, "Could not get gift from first user.");
            User? expected = firstGift!.Receiver;
            Assert.IsNotNull(expected, "Could not get user from first gift.");

            Assert.AreEqual<string>(expected!.FirstName, actual.FirstName, "Users dont match.");
        }

        [TestMethod]
        public void MyTestMethod()
        {
            using DbContext dbContext = new(ContextOptions);
            dbContext.Database.Migrate();

            Group? expected = dbContext.Groups.FirstOrDefault<Group>();
            Group? actual = dbContext.Users.FirstOrDefault<User>()?.UserGroups.FirstOrDefault<Group>();

            Assert.AreEqual<string>(expected.Name, actual?.Name);
        }
    }
}
