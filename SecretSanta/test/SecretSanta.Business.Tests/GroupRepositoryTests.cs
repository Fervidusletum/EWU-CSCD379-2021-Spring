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
    public class GroupRepositoryTests : TestableDbContext
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Create_NullItem_ThrowsArgumentException()
        {
            using DbContext dbContext = new(ContextOptions);
            await Init(dbContext);
            GroupRepository sut = new(dbContext);

            sut.Create(null!);
        }

        [TestMethod]
        public async Task Create_WithItem_CanGetItem()
        {
            using DbContext dbContext = new(ContextOptions);
            await Init(dbContext);
            GroupRepository sut = new(dbContext);
            Group group = new()
            {
                Id = 42,
                Name = "testgroup"
            };

            Group createdGroup = sut.Create(group);

            Group? retrievedGroup = sut.GetItem(createdGroup.Id);
            Assert.AreEqual(group, retrievedGroup);
        }

        [TestMethod]
        public async Task GetItem_WithBadId_ReturnsNull()
        {
            using DbContext dbContext = new(ContextOptions);
            await Init(dbContext);
            GroupRepository sut = new(dbContext);

            Group? user = sut.GetItem(-1);

            Assert.IsNull(user);
        }

        [TestMethod]
        public async Task GetItem_WithValidId_ReturnsGroup()
        {
            using DbContext dbContext = new(ContextOptions);
            await Init(dbContext);
            GroupRepository sut = new(dbContext);
            sut.Create(new() 
            { 
                Id = 42,
                Name = "Group",
            });

            Group? user = sut.GetItem(42);

            Assert.AreEqual(42, user?.Id);
            Assert.AreEqual("Group", user!.Name);
        }

        [TestMethod]
        public async Task List_WithGroups_ReturnsAllGroup()
        {
            using DbContext dbContext = new(ContextOptions);
            await Init(dbContext);
            GroupRepository sut = new(dbContext);

            int expected = dbContext.Groups.Count() + 1;
            sut.Create(new()
            {
                Id = 42,
                Name = "Group",
            });

            ICollection<Group> users = sut.List();

            Assert.AreEqual(expected, users.Count);
            foreach(var mockGroup in dbContext.Groups)
            {
                Assert.IsNotNull(users.SingleOrDefault(x => x.Name == mockGroup.Name));
            }
        }

        [TestMethod]
        [DataRow(-1, false)]
        [DataRow(42, true)]
        public async Task Remove_WithInvalidId_ReturnsTrue(int id, bool expected)
        {
            using DbContext dbContext = new(ContextOptions);
            await Init(dbContext);
            GroupRepository sut = new(dbContext);
            sut.Create(new()
            {
                Id = 42,
                Name = "Group"
            });

            Assert.AreEqual(expected, sut.Remove(id));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Save_NullItem_ThrowsArgumentException()
        {
            using DbContext dbContext = new(ContextOptions);
            await Init(dbContext);
            GroupRepository sut = new(dbContext);

            sut.Save(null!);
        }

        [TestMethod]
        public async Task Save_WithValidItem_SavesItem()
        {
            using DbContext dbContext = new(ContextOptions);
            await Init(dbContext);
            GroupRepository sut = new(dbContext);

            Group fake = new Group() { Id = 42, Name = "testgroup" };
            sut.Create(fake);

            string expected = "updatedname";
            fake.Name = expected;

            Assert.AreEqual(expected, sut.GetItem(42)?.Name);
        }

        [TestMethod]
        public async Task GenerateAssignments_WithInvalidId_ReturnsError()
        {
            using DbContext dbContext = new(ContextOptions);
            await Init(dbContext);
            GroupRepository sut = new(dbContext);

            AssignmentResult result = sut.GenerateAssignments(42);

            Assert.AreEqual("Group not found", result.ErrorMessage);
        }

        [TestMethod]
        public async Task GenerateAssignments_WithLessThanThreeUsers_ReturnsError()
        {
            using DbContext dbContext = new(ContextOptions);
            await Init(dbContext);
            GroupRepository sut = new(dbContext);
            sut.Create(new()
            {
                Id = 42,
                Name = "Group"
            });

            AssignmentResult result = sut.GenerateAssignments(42);

            Assert.AreEqual($"Group Group must have at least three users", result.ErrorMessage);
        }

        [TestMethod]
        public async Task GenerateAssignments_WithValidGroup_CreatesAssignments()
        {
            using DbContext dbContext = new(ContextOptions);
            await Init(dbContext);
            GroupRepository sut = new(dbContext);
            Group group = new()
            {
                Id = 42,
                Name = "Group"
            };
            group.Users.Add(new User { FirstName = "John", LastName = "Doe" });
            group.Users.Add(new User { FirstName = "Jane", LastName = "Smith" });
            group.Users.Add(new User { FirstName = "Bob", LastName = "Jones" });
            sut.Create(group);

            AssignmentResult result = sut.GenerateAssignments(42);
            Group actual = sut.GetItem(42)!;

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(3, actual.Assignments.Count);
            Assert.AreEqual(3, actual.Assignments.Select(x => x.Giver.FirstName).Distinct().Count());
            Assert.AreEqual(3, actual.Assignments.Select(x => x.Receiver.FirstName).Distinct().Count());
            Assert.IsFalse(actual.Assignments.Any(x => x.Giver.FirstName == x.Receiver.FirstName));
        }
    }
}
