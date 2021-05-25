using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSanta.Data;

namespace SecretSanta.Business.Tests
{
    [TestClass]
    public class GroupRepositoryTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Create_NullItem_ThrowsArgumentException()
        {
            GroupRepository sut = new();

            sut.Create(null!);
        }

        [TestMethod]
        public void Create_WithItem_CanGetItem()
        {
            GroupRepository sut = new();
            Group user = new()
            {
                Id = 42
            };

            Group createdGroup = sut.Create(user);

            Group? retrievedGroup = sut.GetItem(createdGroup.Id);
            Assert.AreEqual(user, retrievedGroup);
        }

        [TestMethod]
        public void GetItem_WithBadId_ReturnsNull()
        {
            GroupRepository sut = new();

            Group? user = sut.GetItem(-1);

            Assert.IsNull(user);
        }

        [TestMethod]
        public void GetItem_WithValidId_ReturnsGroup()
        {
            GroupRepository sut = new();
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
        public void List_WithGroups_ReturnsAllGroup()
        {
            GroupRepository sut = new();
            sut.Create(new()
            {
                Id = 42,
                Name = "Group",
            });

            ICollection<Group> users = sut.List();

            Assert.AreEqual(MockData.Groups.Count, users.Count);
            foreach(var mockGroup in MockData.Groups.Values)
            {
                Assert.IsNotNull(users.SingleOrDefault(x => x.Name == mockGroup.Name));
            }
        }

        [TestMethod]
        [DataRow(-1, false)]
        [DataRow(42, true)]
        public void Remove_WithInvalidId_ReturnsTrue(int id, bool expected)
        {
            GroupRepository sut = new();
            sut.Create(new()
            {
                Id = 42,
                Name = "Group"
            });

            Assert.AreEqual(expected, sut.Remove(id));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Save_NullItem_ThrowsArgumentException()
        {
            GroupRepository sut = new();

            sut.Save(null!);
        }

        [TestMethod]
        public void Save_WithValidItem_SavesItem()
        {
            GroupRepository sut = new();

            sut.Save(new Group() { Id = 42 });

            Assert.AreEqual(42, sut.GetItem(42)?.Id);
        }

        [TestMethod]
        public void GenerateAssignments_WithInvalidId_ReturnsError()
        {
            GroupRepository sut = new();

            // idx 42 was causing intermittent failure of test when certain other tests had been run
            AssignmentResult result = sut.GenerateAssignments(420);

            Assert.AreEqual("Group not found", result.ErrorMessage);
        }

        [TestMethod]
        public void GenerateAssignments_WithLessThanThreeUsers_ReturnsError()
        {
            GroupRepository sut = new();
            sut.Create(new()
            {
                Id = 42,
                Name = "Group"
            });

            AssignmentResult result = sut.GenerateAssignments(42);

            Assert.AreEqual($"Group Group must have at least three users", result.ErrorMessage);
        }

        [TestMethod]
        public void GenerateAssignments_WithValidGroup_CreatesAssignments()
        {
            GroupRepository sut = new();
            Group group = sut.Create(new()
            {
                Id = 42,
                Name = "Group"
            });
            group.Users.Add(new User { FirstName = "John", LastName = "Doe", Id = 1 });
            group.Users.Add(new User { FirstName = "Jane", LastName = "Smith", Id = 2 });
            group.Users.Add(new User { FirstName = "Bob", LastName = "Jones", Id = 3 });

            AssignmentResult result = sut.GenerateAssignments(42);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(3, group.Assignments.Count);
            Assert.AreEqual(3, group.Assignments.Select(x => x.Giver.FirstName).Distinct().Count());
            Assert.AreEqual(3, group.Assignments.Select(x => x.Receiver.FirstName).Distinct().Count());
            Assert.IsFalse(group.Assignments.Any(x => x.Giver.FirstName == x.Receiver.FirstName));
        }

        [TestMethod]
        public void GenerateAssignments_UsingGivenOrderingFunction_ShufflesAssignmentsInGivenOrder()
        {
            GroupRepository.Shuffler shuffle = (ICollection<User> users) => users.AsEnumerable().Reverse().ToList();
            GroupRepository sut = new(shuffle);
            Group group = sut.Create(new()
            {
                Id = 42,
                Name = "Group"
            });
            group.Users.Add(new User { FirstName = "John", LastName = "Doe", Id = 1 });
            group.Users.Add(new User { FirstName = "Jane", LastName = "Smith", Id = 2 });
            group.Users.Add(new User { FirstName = "Bob", LastName = "Jones", Id = 3 });

            // because I want to use the linq reverse, which does not operate in place
            List<User> expected = group.Users.AsEnumerable().Reverse().ToList();
            expected.Add(expected.First()); // avoiding modulo logic for circular linking

            AssignmentResult result = sut.GenerateAssignments(42);

            for(int i = 0; i < expected.Count-1; i++)
            {
                Assert.AreEqual<int>(expected[i].Id, group.Assignments[i].Giver.Id, $"expected {i} does not match Assignment {i} Giver");
                Assert.AreEqual<int>(expected[i+1].Id, group.Assignments[i].Receiver.Id, $"expected {i+1} does not match Assignment {i} Receiver");
            }
        }
    }
}
