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
        // adapted from from https://docs.microsoft.com/en-us/ef/core/testing/sqlite#using-sqlite-in-memory-databases
        // and https://docs.microsoft.com/en-us/ef/core/testing/testing-sample#the-tests
        // and https://github.com/dotnet/EntityFramework.Docs/blob/main/samples/core/Miscellaneous/Testing/ItemsWebApi/ItemsWebApi/ItemsContext.cs

        private DbConnection Connection { get; }
        protected DbContextOptions<DbContext> ContextOptions { get; }
        protected string SeedStr { get; } = "TestSeed";

        public DbContextTests()
        {
            ContextOptions = new DbContextOptionsBuilder<DbContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .Options;
            Connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
            //Seed(); // not sure why, but seeding from constructor isnt populating data correctly (this was before making it async with param)
            // whereas it's demonstrated in the example at https://docs.microsoft.com/en-us/ef/core/testing/testing-sample#setting-up-and-seeding-the-database
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        }

        public void Dispose() => Connection.Dispose();

        // since this is an in-memory db, it should be fine to repeatedly delete and repopulate it for each test
        async private Task Seed(DbContext dbContext)
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.Migrate();

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

            testGroup.Assignments.Add(new Assignment(users[0], users[1]));
            testGroup.Assignments.Add(new Assignment(users[1], users[2]));
            testGroup.Assignments.Add(new Assignment(users[2], users[0]));

            dbContext.Groups.Add(testGroup);
            await dbContext.SaveChangesAsync();
        }

        #endregion

        #region Auto-Populate Tests

        [TestMethod]
        async public Task DbContext_DataIsSeeded_GroupPopulated()
        {
            using DbContext dbContext = new(ContextOptions);
            await Seed(dbContext);

            Group? actual = dbContext.Groups.FirstOrDefault<Group>();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        async public Task Gift_ReceiverAutoPopulated_MatchesUser()
        {
            using DbContext dbContext = new(ContextOptions);
            await Seed(dbContext);

            User expected = dbContext.Users.First<User>();
            User? actual = expected.Gifts.First<Gift>().Receiver;

            Assert.IsNotNull(actual, $"{nameof(Gift.Receiver)} is not populated.");
            Assert.AreEqual<string>(expected.FirstName, actual.FirstName, "Users dont match.");
        }

        [TestMethod]
        async public Task Gift_AutoPopulatedInDbContext_MatchesGiftFromUser()
        {
            using DbContext dbContext = new(ContextOptions);
            await Seed(dbContext);

            Gift expected = dbContext.Users.First<User>().Gifts.First<Gift>();
            Gift? actual = dbContext.Gifts.FirstOrDefault<Gift>(g => g.Id == expected.Id);

            Assert.IsNotNull(actual, $"{nameof(Gift)} not found.");
            Assert.AreEqual<string>(expected.Title, actual!.Title, $"{nameof(Gift)} don't match.");
        }

        [TestMethod]
        async public Task User_AutoPopulatedInDbContext_MatchesFromGroup()
        {
            using DbContext dbContext = new(ContextOptions);
            await Seed(dbContext);

            User expected = dbContext.Groups.First<Group>().Users.First<User>();
            User? actual = dbContext.Users.FirstOrDefault<User>(u => u.Id == expected.Id);

            Assert.IsNotNull(actual);
            Assert.AreEqual<string>(expected.FirstName, actual!.FirstName);
        }


        [TestMethod]
        async public Task Group_AutoPopulatedInUser_MatchesFromUser()
        {
            using DbContext dbContext = new(ContextOptions);
            await Seed(dbContext);

            Group expected = dbContext.Groups.First<Group>();
            Group? actual = dbContext.Users.First<User>().Groups.FirstOrDefault<Group>();

            Assert.IsNotNull(actual);
            Assert.AreEqual<string>(expected.Name, actual!.Name);
        }

        #endregion

        #region DB Manipulation Tests

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

        #endregion
    }
}
