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


        #region GET() TESTS
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
        #endregion GET() TESTS


        #region GET(ID) TESTS
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
        #endregion GET(ID) TESTS


        #region POST(DTO) TESTS
        [TestMethod]
        public async Task Post_GivenValidUser_ReturnsValidDto()
        {
            TestableUserRepository repo = Factory.Repo;
            UserDtoFull newUser = new() { Id = 1, FirstName = "fn", LastName = "ln" };
            User expected = new() { Id = (int)newUser.Id, FirstName = newUser.FirstName, LastName = newUser.LastName };
            repo.CreateReturnUser = expected;
            HttpClient client = Factory.CreateClient();

            HttpResponseMessage response = await client.PostAsJsonAsync("/api/users/", newUser);
            UserDtoFull? content = await response.Content.ReadFromJsonAsync<UserDtoFull?>();

            response.EnsureSuccessStatusCode();
            Assert.AreEqual<int>(expected.Id, content?.Id ?? -1);
            Assert.AreEqual<string>(expected.FirstName, content!.FirstName!);
            Assert.AreEqual<string>(expected.LastName, content!.LastName!);
        }

        [TestMethod]
        public async Task Post_GivenInvalidUser_Returns400()
        {
            HttpClient client = Factory.CreateClient();

            HttpResponseMessage response = await client.PostAsJsonAsync<UserDtoFull>("/api/users/", null!);

            Assert.AreEqual(HttpStatusCode.BadRequest.GetTypeCode(), response.StatusCode.GetTypeCode());
        }

        [TestMethod]
        public async Task Post_GivenNullId_Returns400()
        {
            HttpClient client = Factory.CreateClient();

            HttpResponseMessage response = await client.PostAsJsonAsync("/api/users/",
                new UserDtoFull { Id = null!, FirstName = "", LastName = "" }
            );

            Assert.AreEqual(HttpStatusCode.BadRequest.GetTypeCode(), response.StatusCode.GetTypeCode());
        }
        #endregion POST(DTO) TESTS


        #region PUT(ID,DTO) TESTS
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
            Assert.AreEqual<int>(updateId, repo.GetItemParamId);
            Assert.AreEqual<string>(updateUser.FirstName, repo.SaveParamItem?.FirstName);
            Assert.AreEqual<string>(updateUser.LastName, repo.SaveParamItem?.LastName);
        }

        [TestMethod]
        public async Task Put_GivenInvalidUser_Returns400()
        {
            HttpClient client = Factory.CreateClient();

            HttpResponseMessage response = await client.PutAsJsonAsync<UserDtoFnLn>("/api/users/0", null!);

            Assert.AreEqual(HttpStatusCode.BadRequest.GetTypeCode(), response.StatusCode.GetTypeCode());
        }

        [TestMethod]
        public async Task Put_GivenInvalidId_Returns404()
        {
            HttpClient client = Factory.CreateClient();
            Factory.Repo.GetItemReturnUser = null!;

            HttpResponseMessage response = await client.PutAsJsonAsync("/api/users/0", new User { FirstName = "f", LastName = "l" });

            Assert.AreEqual(HttpStatusCode.NotFound.GetTypeCode(), response.StatusCode.GetTypeCode());
        }
        #endregion PUT(ID,DTO) TESTS


        #region DELETE(ID) TESTS
        [TestMethod]
        public async Task Delete_GivenValidId_RemovesAppropriateUser()
        {
            TestableUserRepository repo = Factory.Repo;
            repo.RemoveReturnBool = true;
            HttpClient client = Factory.CreateClient();
            int removeId = 5;

            HttpResponseMessage response = await client.DeleteAsync("/api/users/"+removeId);

            response.EnsureSuccessStatusCode();
            Assert.AreEqual<int>(removeId, repo.RemoveParamId);
        }

        [TestMethod]
        public async Task Delete_GivenInvalidId_Returns404()
        {
            TestableUserRepository repo = Factory.Repo;
            repo.RemoveReturnBool = false;
            HttpClient client = Factory.CreateClient();

            HttpResponseMessage response = await client.DeleteAsync("/api/users/0");

            Assert.AreEqual(HttpStatusCode.NotFound.GetTypeCode(), response.StatusCode.GetTypeCode());
        }
        #endregion DELETE(ID) TESTS
    }
}
