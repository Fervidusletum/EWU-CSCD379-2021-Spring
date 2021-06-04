using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SecretSanta.Data;

namespace SecretSanta.Data.Tests
{
    [TestClass]
    public class DbContextTests
    {
        /*
        [ClassInitialize]
        private void Seed()
        {
            using DbContext dbContext = new DbContext();
            dbContext.Database.Migrate();

            User testUser = dbContext.Users.FirstOrDefault
        }
        */

        [TestMethod]
        async public Task DbContext_AddingUser_CountIncrementsByOne()
        {
            using DbContext dbContext = new DbContext();
            string fn = Guid.NewGuid().ToString();
            string ln = nameof(DbContextTests) + nameof(DbContext_AddingUser_CountIncrementsByOne);

            async Task RemoveExistingTestUserAsync()
            {
                IQueryable<User>? users = dbContext.Users.Where(
                    user => user.LastName == ln);
                dbContext.Users.RemoveRange(users);
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
            using DbContext dbContext = new DbContext();
            string fn = Guid.NewGuid().ToString();
            string ln = nameof(DbContextTests) + nameof(DbContext_AddingUser_CountIncrementsByOne);

            async Task RemoveExistingTestUserAsync()
            {
                IQueryable<User>? users = dbContext.Users.Where(
                    user => user.LastName == ln);
                dbContext.Users.RemoveRange(users);
                await dbContext.SaveChangesAsync();
            }

            try
            {
                await RemoveExistingTestUserAsync();
                dbContext.Users.Add(new User { FirstName = fn, LastName = ln });
                await dbContext.SaveChangesAsync();
                int expected = dbContext.Users.Count() - 1;

                IQueryable<User>? target = dbContext.Users.Where(u => u.FirstName == fn && u.LastName == ln);
                dbContext.Users.RemoveRange(target);
                await dbContext.SaveChangesAsync();

                Assert.AreEqual<int>(expected, dbContext.Users.Count());
            }
            finally
            {
                await RemoveExistingTestUserAsync();
            }
        }

        [TestMethod]
        async public Task DbContext_AccessUser_HasExpectedValues()
        {
            using DbContext dbContext = new DbContext();
            string fn = Guid.NewGuid().ToString();
            string ln = nameof(DbContextTests) + nameof(DbContext_AddingUser_CountIncrementsByOne);

            async Task RemoveExistingTestUserAsync()
            {
                IQueryable<User>? users = dbContext.Users.Where(
                    user => user.LastName == ln);
                dbContext.Users.RemoveRange(users);
                await dbContext.SaveChangesAsync();
            }

            try
            {
                await RemoveExistingTestUserAsync();
                dbContext.Users.Add(new User { FirstName = fn, LastName = ln });
                await dbContext.SaveChangesAsync();

                User? actual = dbContext.Users.Where(u => u.FirstName == fn && u.LastName == ln).FirstOrDefault();

                Assert.IsNotNull(actual);
                Assert.AreEqual<string>(fn, actual!.FirstName);
                Assert.AreEqual<string>(ln, actual!.LastName);
            }
            finally
            {
                await RemoveExistingTestUserAsync();
            }
        }
    }
}
