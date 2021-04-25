using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using SecretSanta.Business;
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
            IUserRepository sut = new UserRepository(new MockCollection());

            ICollection<User> users = sut.List();

            Assert.IsTrue(users.Any());
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void GetItem_GivenValidId_ReturnsCorrectUser(int id)
        {
            MockCollection mockdata = new MockCollection();
            IUserRepository sut = new UserRepository(mockdata);

            User user = sut.GetItem(id) ?? throw new IndexOutOfRangeException(nameof(id));

            Assert.AreEqual(mockdata[id].Id, user.Id);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(4)]
        public void GetItem_GivenBadId_ReturnsNull(int id)
        {
            IUserRepository sut = new UserRepository(new MockCollection());

            User? user = sut.GetItem(id);

            Assert.IsNull(user);
        }

        [TestMethod]
        public void Create_GivenValidUser_AddsToList()
        {
            MockCollection mockdata = new MockCollection();
            IUserRepository sut = new UserRepository(mockdata);
            User fakeUser = new User() { Id = 3, FirstName = "test", LastName = "ing" };

            sut.Create(fakeUser);

            Assert.AreEqual(fakeUser.Id, mockdata[3].Id);
            Assert.AreEqual(fakeUser.FirstName, mockdata[3].FirstName);
            Assert.AreEqual(fakeUser.LastName, mockdata[3].LastName);
        }

        [TestMethod]
        public void Create_GivenNullUser_ThrowsArgNullException()
        {
            IUserRepository sut = new UserRepository(new MockCollection());

            ArgumentNullException ex = Assert.ThrowsException<ArgumentNullException>(
                () => sut.Create(null!));

            Assert.AreEqual("newUser", ex.ParamName);
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(0)]
        public void Remove_GivenValidId_RemovesUser(int id)
        {
            MockCollection mockdata = new MockCollection();
            IUserRepository sut = new UserRepository(mockdata);
            int count = mockdata.Count;
            User removed = mockdata[id];

            bool status = sut.Remove(id);

            Assert.IsTrue(status);
            Assert.AreEqual(count - 1, mockdata.Count);
            Assert.IsNull(mockdata.FirstOrDefault(user => user.Id == removed.Id));
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(3)]
        public void Remove_GivenInvalidId_ReturnsFalse(int id)
        {
            MockCollection mockdata = new MockCollection();
            IUserRepository sut = new UserRepository(mockdata);
            int count = mockdata.Count;

            bool status = sut.Remove(id);

            Assert.IsFalse(status);
            Assert.AreEqual(count, mockdata.Count);
        }

        [TestMethod]
        public void Save_GivenValidChangedUser_UpdatesUser()
        {
            MockCollection mockdata = new MockCollection();
            IUserRepository sut = new UserRepository(mockdata);
            User fakeUser = new User() { Id = 0, FirstName = "test", LastName = "ing" };

            sut.Save(fakeUser);

            User updatedUser = mockdata.Find(user => user.Id == fakeUser.Id)!;
            Assert.AreEqual(fakeUser.Id, updatedUser.Id);
            Assert.AreEqual(fakeUser.FirstName, updatedUser.FirstName);
            Assert.AreEqual(fakeUser.LastName, updatedUser.LastName);
        }

        [TestMethod]
        public void Save_GivenNullUser_ThrowsArgNullException()
        {
            IUserRepository sut = new UserRepository(new MockCollection());

            ArgumentNullException ex = Assert.ThrowsException<ArgumentNullException>(
                () => sut.Save(null!));

            Assert.AreEqual("user", ex.ParamName);
        }


        private class MockCollection : List<User>
        {
            public MockCollection()
            {
                Add(new User() { Id = 0, FirstName = "te", LastName = "st" });
                Add(new User() { Id = 1, FirstName = "fn", LastName = "ln" });
                Add(new User() { Id = 2, FirstName = "da", LastName = "ta" });
            }
        }
    }
}
