using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSanta.Data;

namespace SecretSanta.Business.Tests
{
    [TestClass]
    public class UserRepositoryTests
    {
        [TestMethod]
        public void Constructor_GivenNullCollection_ThrowsArgNullException()
        {
            ArgumentNullException ex = Assert.ThrowsException<ArgumentNullException>(
                () => new UserRepository(null!));
            Assert.AreEqual("userCollection", ex.ParamName);
        }

        [TestMethod]
        public void List_Request_ReturnsUsers()
        {
            IUserRepository sut = new UserRepository(new FakeCollection());

            ICollection<User> users = sut.List();

            Assert.IsTrue(users.Any());
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void GetItem_GivenValidId_ReturnsCorrectUser(int id)
        {
            FakeCollection fakedata = new FakeCollection();
            IUserRepository sut = new UserRepository(fakedata);

            User user = sut.GetItem(id) ?? throw new IndexOutOfRangeException(nameof(id));

            Assert.AreEqual(fakedata[id].Id, user.Id);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(4)]
        public void GetItem_GivenInvalidId_ReturnsNull(int id)
        {
            IUserRepository sut = new UserRepository(new FakeCollection());

            User? user = sut.GetItem(id);

            Assert.IsNull(user);
        }

        [TestMethod]
        public void Create_GivenValidUser_AddsToList()
        {
            FakeCollection fakedata = new FakeCollection();
            IUserRepository sut = new UserRepository(fakedata);
            User fakeUser = new User() { Id = 3, FirstName = "test", LastName = "ing" };

            sut.Create(fakeUser);

            Assert.AreEqual(fakeUser.Id, fakedata[3].Id);
            Assert.AreEqual(fakeUser.FirstName, fakedata[3].FirstName);
            Assert.AreEqual(fakeUser.LastName, fakedata[3].LastName);
        }

        [TestMethod]
        public void Create_GivenNullUser_ThrowsArgNullException()
        {
            IUserRepository sut = new UserRepository(new FakeCollection());

            ArgumentNullException ex = Assert.ThrowsException<ArgumentNullException>(
                () => sut.Create(null!));

            Assert.AreEqual("newUser", ex.ParamName);
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(0)]
        public void Remove_GivenValidId_RemovesUser(int id)
        {
            FakeCollection fakedata = new FakeCollection();
            IUserRepository sut = new UserRepository(fakedata);
            int count = fakedata.Count;
            User removed = fakedata[id];

            bool status = sut.Remove(id);

            Assert.IsTrue(status);
            Assert.AreEqual(count - 1, fakedata.Count);
            Assert.IsNull(fakedata.FirstOrDefault(user => user.Id == removed.Id));
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(3)]
        public void Remove_GivenInvalidId_ReturnsFalse(int id)
        {
            FakeCollection fakedata = new FakeCollection();
            IUserRepository sut = new UserRepository(fakedata);
            int count = fakedata.Count;

            bool status = sut.Remove(id);

            Assert.IsFalse(status);
            Assert.AreEqual(count, fakedata.Count);
        }

        [TestMethod]
        public void Save_GivenValidChangedUser_UpdatesUser()
        {
            FakeCollection fakedata = new FakeCollection();
            IUserRepository sut = new UserRepository(fakedata);
            User fakeUser = new User() { Id = 0, FirstName = "test", LastName = "ing" };

            sut.Save(fakeUser);

            User updatedUser = fakedata.Find(user => user.Id == fakeUser.Id)!;
            Assert.AreEqual(fakeUser.Id, updatedUser.Id);
            Assert.AreEqual(fakeUser.FirstName, updatedUser.FirstName);
            Assert.AreEqual(fakeUser.LastName, updatedUser.LastName);
        }

        [TestMethod]
        public void Save_GivenNullUser_ThrowsArgNullException()
        {
            IUserRepository sut = new UserRepository(new FakeCollection());

            ArgumentNullException ex = Assert.ThrowsException<ArgumentNullException>(
                () => sut.Save(null!));

            Assert.AreEqual("user", ex.ParamName);
        }


        private class FakeCollection : List<User>
        {
            public FakeCollection()
            {
                Add(new User() { Id = 0, FirstName = "te", LastName = "st" });
                Add(new User() { Id = 1, FirstName = "fn", LastName = "ln" });
                Add(new User() { Id = 2, FirstName = "da", LastName = "ta" });
            }
        }
    }
}
