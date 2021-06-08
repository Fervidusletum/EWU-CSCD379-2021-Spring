using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSanta.Data;
using SecretSanta.Data.Tests;

namespace SecretSanta.Business.Tests
{
    [TestClass]
    public class UserRepositoryTests : TestableDbContext
    {
        // bad practice to make a test double for dbcontext
        // instead giving dbcontext an in-mem db designed for testing

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Create_NullItem_ThrowsArgumentException()
        {
            using DbContext dbContext = new(ContextOptions);
            Init(dbContext);
            UserRepository sut = new(dbContext);

            sut.Create(null!);
        }

        [TestMethod]
        public void Create_WithItem_CanGetItem()
        {
            using DbContext dbContext = new(ContextOptions);
            Init(dbContext);
            UserRepository sut = new(dbContext);
            User user = new()
            {
                Id = 42
            };

            User createdUser = sut.Create(user);

            User? retrievedUser = sut.GetItem(createdUser.Id);
            Assert.AreEqual(user, retrievedUser);
        }

        [TestMethod]
        public void GetItem_WithBadId_ReturnsNull()
        {
            using DbContext dbContext = new(ContextOptions);
            Init(dbContext);
            UserRepository sut = new(dbContext);

            User? user = sut.GetItem(-1);

            Assert.IsNull(user);
        }

        [TestMethod]
        public void GetItem_WithValidId_ReturnsUser()
        {
            using DbContext dbContext = new(ContextOptions);
            Init(dbContext);
            UserRepository sut = new(dbContext);
            sut.Create(new() 
            { 
                Id = 42,
                FirstName = "First",
                LastName = "Last"
            });

            User? user = sut.GetItem(42);

            Assert.AreEqual(42, user?.Id);
            Assert.AreEqual("First", user!.FirstName);
            Assert.AreEqual("Last", user.LastName);
        }

        [TestMethod]
        public void List_WithUsers_ReturnsAllUser()
        {
            using DbContext dbContext = new(ContextOptions);
            Init(dbContext);
            UserRepository sut = new(dbContext);

            int expected = dbContext.Users.Count() + 1;
            sut.Create(new()
            {
                Id = 42,
                FirstName = "First",
                LastName = "Last"
            });

            ICollection<User> users = sut.List();

            Assert.AreEqual(expected, users.Count);
            foreach(var mockUser in dbContext.Users)
            {
                Assert.IsNotNull(users.SingleOrDefault(x => x.FirstName == mockUser.FirstName && x.LastName == mockUser.LastName));
            }
        }

        [TestMethod]
        [DataRow(-1, false)]
        [DataRow(42, true)]
        public void Remove_WithInvalidId_ReturnsTrue(int id, bool expected)
        {
            using DbContext dbContext = new(ContextOptions);
            Init(dbContext);
            UserRepository sut = new(dbContext);
            sut.Create(new()
            {
                Id = 42,
                FirstName = "First",
                LastName = "Last"
            });

            Assert.AreEqual(expected, sut.Remove(id));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Save_NullItem_ThrowsArgumentException()
        {
            using DbContext dbContext = new(ContextOptions);
            Init(dbContext);
            UserRepository sut = new(dbContext);

            sut.Save(null!);
        }

        [TestMethod]
        public void Save_WithValidItem_SavesItem()
        {
            using DbContext dbContext = new(ContextOptions);
            Init(dbContext);
            UserRepository sut = new(dbContext);

            User fake = new User() { Id = 42, FirstName = "test", LastName = "user" };
            sut.Create(fake);

            string expected = "updated";
            fake.FirstName = expected;

            sut.Save(fake);

            Assert.AreEqual(expected, sut.GetItem(42)?.FirstName);
        }
    }
}
