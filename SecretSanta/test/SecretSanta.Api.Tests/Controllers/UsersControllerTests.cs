using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSanta.Api.Controllers;
using SecretSanta.Api.Dto;
using SecretSanta.Business;
using SecretSanta.Data;

namespace SecretSanta.Api.Tests.Controllers
{
    [TestClass]
    public class UsersControllerTests
    {
        [TestMethod]
        public void Constructor_GivenNullRepo_ThrowsArgNullException()
        {
            ArgumentNullException ex = Assert.ThrowsException<ArgumentNullException>(
                () => new UsersController(null!));
            Assert.AreEqual("userRepo", ex.ParamName);
        }

        [TestMethod]
        public void Get_GivenNoArgs_ReturnsUsersList()
        {
            mockRepo mock = new(
                new List<User>() { new User() { Id = 0, FirstName = "test", LastName = "ing" }
            });
            UsersController sut = new(mock);

            ICollection<User> users = sut.Get();

            Assert.IsTrue(users.Any());
        }

        [TestMethod]
        [DataRow(0)]
        public void Get_GivenValidId_ReturnsUser(int id)
        {
            User expectedUser = new User() { Id = 0, FirstName = "test", LastName = "ing" };
            mockRepo mock = new();
            mock.GetItemUser = expectedUser;
            UsersController sut = new(mock);

            ActionResult<User> result = sut.Get(id);

            Assert.AreEqual(expectedUser, result.Value);
        }

        [TestMethod]
        public void Get_GivenInvalidId_ReturnsNotFound()
        {
            mockRepo mock = new();
            mock.GetItemUser = null!;
            UsersController sut = new(mock);

            ActionResult<User> result = sut.Get(0);

            Assert.IsTrue(result.Result is NotFoundResult);
        }

        [TestMethod]
        public void Delete_GivenValidId_ReturnsOk()
        {
            mockRepo mock = new();
            mock.RemoveSuccess = true;
            UsersController sut = new(mock);

            ActionResult result = sut.Delete(0);

            Assert.IsTrue(result is OkResult);
        }

        [TestMethod]
        public void Delete_GivenInvalidId_ReturnsNotFound()
        {
            mockRepo mock = new();
            mock.RemoveSuccess = false;
            UsersController sut = new(mock);

            ActionResult result = sut.Delete(0);

            Assert.IsTrue(result is NotFoundResult);
        }

        [TestMethod]
        public void Post_GivenValidUser_ReturnsUser()
        {
            User expectedUser = new User() { Id = 0, FirstName = "test", LastName = "ing" };
            mockRepo mock = new();
            UsersController sut = new(mock);

            ActionResult<User?> result = sut.Post(expectedUser);

            Assert.AreEqual(expectedUser, result.Value);
        }

        [TestMethod]
        public void Post_GivenInvalidUser_ReturnsBadRequest()
        {
            UsersController sut = new(new mockRepo());

            ActionResult<User?> result = sut.Post(null!);

            Assert.IsTrue(result.Result is BadRequestResult);
        }

        [TestMethod]
        public void Put_GivenValidData_UpdatesUser()
        {
            DtoUser update = new() { FirstName = "up", LastName = "date" };
            User olduser = new() { Id = 0, FirstName = "test", LastName = "ing" };
            mockRepo mock = new();
            mock.GetItemUser = olduser;
            UsersController sut = new(mock);

            ActionResult result = sut.Put(0, update);

            Assert.AreEqual(update.FirstName, olduser.FirstName);
            Assert.AreEqual(update.LastName, olduser.LastName);
            Assert.IsTrue(result is OkResult);
        }

        [TestMethod]
        public void Put_GivenInvalidDto_ReturnsBadRequest()
        {
            UsersController sut = new(new mockRepo());

            ActionResult result = sut.Put(0, null!);

            Assert.IsTrue(result is BadRequestResult);
        }

        [TestMethod]
        public void Put_GivenInvalidId_ReturnsNotFound()
        {
            mockRepo mock = new();
            mock.GetItemUser = null!;
            UsersController sut = new(mock);

            ActionResult result = sut.Put(0, new DtoUser());

            Assert.IsTrue(result is NotFoundResult);
        }


        private class mockRepo : IUserRepository
        {
            public ICollection<User>? Users { get; set; }

            public mockRepo() { }
            public mockRepo(ICollection<User> users) => Users = users;

            public ICollection<User> List() => Users!;

            public User? GetItemUser { get; set; }
            public User? GetItem(int id) => GetItemUser;

            public User Create(User item) => item;

            public bool RemoveSuccess { get; set; }
            public bool Remove(int id) => RemoveSuccess;

            public User? SavedUser { get; set; }
            public void Save(User user) => SavedUser = user;
        }
    }
}
