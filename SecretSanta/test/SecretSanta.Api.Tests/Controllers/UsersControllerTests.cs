using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSanta.Api.Tests.Business;
using SecretSanta.Api.Dto;
using SecretSanta.Data;

namespace SecretSanta.Api.Tests.Controllers
{
    [TestClass]
    public class UsersControllerTests
    {
        private WebApplicationFactory Factory { get; } = new();

        [TestMethod]
        public async Task Get_NoParams_ReturnsValidUsersList()
        {
            TestableUserRepository repo = Factory.Repo;
            List<User> users = new()
            {
                new User { Id = 0, FirstName = "F1", LastName = "L1" },
                new User { Id = 1, FirstName = "F2", LastName = "L2" }
            };
            repo.ListReturnUserCollection.AddRange(users);
            HttpClient client = Factory.CreateClient();

            HttpResponseMessage response = await client.GetAsync("/api/users/");
            List<UserDtoFull?>? content = await response.Content.ReadFromJsonAsync<List<UserDtoFull?>?>();

            response.EnsureSuccessStatusCode();
            Assert.AreEqual<int>(users.Count, content?.Count ?? -1);
            for (int i = 0; i < users.Count; i++)
            {
                UserDtoFull? dto = content!.FirstOrDefault(userdto => userdto?.Id == users[i].Id);
                Assert.IsNotNull(dto);
                Assert.AreEqual<string>(users[i].FirstName!, dto!.FirstName!);
                Assert.AreEqual<string>(users[i].LastName!, dto!.LastName!);
            }
        }

        [TestMethod]
        public async Task Get_GivenValidId_ReturnsValidDto()
        {
            TestableUserRepository repo = Factory.Repo;
            User user = new User { Id = 1, FirstName = "F2", LastName = "L2" };
            repo.GetItemReturnUser = user;
            HttpClient client = Factory.CreateClient();

            HttpResponseMessage response = await client.GetAsync("/api/users/"+user.Id);
            UserDtoFnLn? content = await response.Content.ReadFromJsonAsync<UserDtoFnLn?>();

            response.EnsureSuccessStatusCode();
            Assert.AreEqual<int>(user.Id, repo.GetItemParamId);
            Assert.AreEqual<string>(user.FirstName, content?.FirstName!);
            Assert.AreEqual<string>(user.LastName, content?.LastName!);
        }

        [TestMethod]
        public async Task Get_GivenInvalidId_Returns404()
        {
            TestableUserRepository repo = Factory.Repo;
            repo.GetItemReturnUser = null!;
            HttpClient client = Factory.CreateClient();

            HttpResponseMessage response = await client.GetAsync("/api/users/0");

            Assert.AreEqual(HttpStatusCode.NotFound.GetTypeCode(), response.StatusCode.GetTypeCode());
        }

        [TestMethod]
        public async Task Post_GivenValidUser_CreatesUser()
        {
            TestableUserRepository repo = Factory.Repo;
            Assert.Fail();
        }

        [TestMethod]
        public async Task Post_GivenValidUser_ReturnsValidDto()
        {
            TestableUserRepository repo = Factory.Repo;
            Assert.Fail();
        }

        [TestMethod]
        public async Task Post_GivenInvalidUser_Returns400()
        {
            HttpClient client = Factory.CreateClient();

            HttpResponseMessage response = await client.PutAsJsonAsync<UserDtoFull>("/api/users/", null!);

            Assert.AreEqual(HttpStatusCode.BadRequest.GetTypeCode(), response.StatusCode.GetTypeCode());
        }

        [TestMethod]
        public async Task Post_GivenNullId_Returns400()
        {
            HttpClient client = Factory.CreateClient();

            HttpResponseMessage response = await client.PutAsJsonAsync("/api/users/",
                new UserDtoFull { Id = null!, FirstName = "", LastName = "" }
            );

            Assert.AreEqual(HttpStatusCode.BadRequest.GetTypeCode(), response.StatusCode.GetTypeCode());
        }

        [TestMethod]
        public async Task Put_GivenValidData_UpdatesUser()
        {
            TestableUserRepository repo = Factory.Repo;

            int updateId = 2;
            repo.GetItemReturnUser = new User { Id = updateId, FirstName = "Test", LastName = "User" };
            UserDtoFnLn updateUser = new() { FirstName = "Up", LastName = "Date" };
            HttpClient client = Factory.CreateClient();

            HttpResponseMessage response = await client.PutAsJsonAsync("/api/users/"+updateId, updateUser);

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(updateId, repo.GetItemParamId);
            Assert.AreEqual(updateUser.FirstName, repo.SaveParamItem?.FirstName);
            Assert.AreEqual(updateUser.LastName, repo.SaveParamItem?.LastName);
        }

        [TestMethod]
        public async Task Delete_GivenValidId_RemovesAppropriateUser()
        {
            TestableUserRepository repo = Factory.Repo;
            Assert.Fail();
        }

        [TestMethod]
        public async Task Delete_GivenInvalidId_Returns404()
        {
            TestableUserRepository repo = Factory.Repo;
            repo.RemoveReturnBool = false;
            HttpClient client = Factory.CreateClient();

            HttpResponseMessage response = await client.PostAsJsonAsync("/api/users/",0);

            Assert.AreEqual(HttpStatusCode.NotFound.GetTypeCode(), response.StatusCode.GetTypeCode());
        }
    }
}
