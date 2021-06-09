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
    public class DbContextTests : TestableDbContext
    {
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
        async public Task User_FromGroup_IsNotNull()
        {
            using DbContext dbContext = new(ContextOptions);
            await Seed(dbContext);

            User? actual = dbContext.Groups.First<Group>().Users.First<User>();

            Assert.IsNotNull(actual);
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

        [TestMethod]
        public async Task Group_AutoPopulatedContents_NotEmpty()
        {
            using DbContext dbContext = new(ContextOptions);
            await Seed(dbContext);

            Group sut = dbContext.Groups.First<Group>();

            Assert.AreNotEqual<int>(0, sut.Users?.Count ?? 0, $"No Users in Group {sut.Name}");
            Assert.AreNotEqual<int>(0, sut.Assignments?.Count ?? 0, $"No Assignments in Group {sut.Name}");
        }

        #endregion

        #region DB Manipulation Tests

        [TestMethod]
        async public Task DbContext_AddingUser_CountIncrementsByOne()
        {
            using DbContext dbContext = new(ContextOptions);
            await Seed(dbContext);

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
            await Seed(dbContext);

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
