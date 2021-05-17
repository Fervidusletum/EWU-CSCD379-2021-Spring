using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSanta.Web.Controllers;
using SecretSanta.Web.Tests.Api;
using SecretSanta.Web.ViewModels;

namespace SecretSanta.Web.Tests.Controllers
{
    [TestClass]
    [Ignore("Removed controller logic and replaced it with JS in views.")]
    public class UsersControllersTests
    {
        [TestMethod]
        public void Constructor_WithNullRepository_ThrowException()
        {
            var ex = Assert.ThrowsException<ArgumentNullException>(() => new UsersController(null!));
            Assert.AreEqual("userClient", ex.ParamName);
        }

        [TestMethod]
        public async Task Index_WithUsers_RetrievesUsers()
        {
            using WebApplicationFactory factory = new();
            TestableUsersClient usersClient = factory.Client;

            await usersClient.PostAsync(new Web.Api.User
            {
                Id = 42,
                FirstName = "John",
                LastName = "Smith"
            });

            HttpClient client = factory.CreateClient();

            HttpResponseMessage response = await client.GetAsync("/users/index");

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(1, usersClient.GetAllInvocationCount);
        }

        [TestMethod]
        public async Task Create_WithValidModel_CreatesUser()
        {
            using WebApplicationFactory factory = new();
            TestableUsersClient usersClient = factory.Client;

            HttpClient client = factory.CreateClient();

            var values = new Dictionary<string?, string?>
            {
                { nameof(UserViewModel.Id), "42" },
                { nameof(UserViewModel.FirstName), "John" },
                { nameof(UserViewModel.LastName), "Smith" },
            };
            using FormUrlEncodedContent content = new(values);

            HttpResponseMessage response = await client.PostAsync("/users/create", content);

            response.EnsureSuccessStatusCode();
            Web.Api.User? createdUser = await usersClient.GetAsync(42);
            Assert.AreEqual(42, createdUser.Id);
            Assert.AreEqual("John", createdUser.FirstName);
            Assert.AreEqual("Smith", createdUser.LastName);
            Assert.AreEqual(1, usersClient.PostInvocationCount);
        }

        [TestMethod]
        public async Task Create_WithInvalidModel_CreatesUser()
        {
            using WebApplicationFactory factory = new();
            TestableUsersClient usersClient = factory.Client;

            HttpClient client = factory.CreateClient();

            var values = new Dictionary<string?, string?>
            {
                { nameof(UserViewModel.Id), "42" },
                { nameof(UserViewModel.FirstName), "" }
            };
            using FormUrlEncodedContent content = new(values);

            HttpResponseMessage response = await client.PostAsync("/users/create", content);

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(0, usersClient.PostInvocationCount);
        }

        [TestMethod]
        public async Task Edit_WithUserId_RetrievesUser()
        {
            using WebApplicationFactory factory = new();
            TestableUsersClient usersClient = factory.Client;
            await usersClient.PostAsync(new Web.Api.User
            {
                Id = 42,
                FirstName = "John",
                LastName = "Smith"
            });
            HttpClient client = factory.CreateClient();

            HttpResponseMessage response = await client.GetAsync("/users/edit/42");

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(1, usersClient.GetInvocationCount);
        }

        [TestMethod]
        public async Task Edit_WithValidModel_UpdatesUser()
        {
            using WebApplicationFactory factory = new();
            TestableUsersClient usersClient = factory.Client;
            await usersClient.PostAsync(new Web.Api.User
            {
                Id = 42,
                FirstName = "John",
                LastName = "Smith"
            });
            HttpClient client = factory.CreateClient();

            var values = new Dictionary<string?, string?>
            {
                { nameof(UserViewModel.Id), "42" },
                { nameof(UserViewModel.FirstName), "Jane" },
                { nameof(UserViewModel.LastName), "Doe" },
            };
            using FormUrlEncodedContent content = new(values);
            HttpResponseMessage response = await client.PostAsync("/users/edit", content);

            response.EnsureSuccessStatusCode();
            Web.Api.User? createdUser = await usersClient.GetAsync(42);
            Assert.AreEqual(42, createdUser.Id);
            Assert.AreEqual("Jane", createdUser.FirstName);
            Assert.AreEqual("Doe", createdUser.LastName);
            Assert.AreEqual(1, usersClient.PutInvocationCount);
        }

        [TestMethod]
        public async Task Edit_WithInvalidModel_DoesNotUpdateUser()
        {
            using WebApplicationFactory factory = new();
            TestableUsersClient usersClient = factory.Client;
            await usersClient.PostAsync(new Web.Api.User
            {
                Id = 42,
                FirstName = "John",
                LastName = "Smith"
            });
            HttpClient client = factory.CreateClient();

            var values = new Dictionary<string?, string?>
            {
                { nameof(UserViewModel.Id), "42" },
                { nameof(UserViewModel.FirstName), "" }
            };
            using FormUrlEncodedContent content = new(values);
            HttpResponseMessage response = await client.PostAsync("/users/edit", content);

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(0, usersClient.PutInvocationCount);
        }

        [TestMethod]
        public async Task Delete_WithUserId_RemovesUsers()
        {
            using WebApplicationFactory factory = new();
            TestableUsersClient usersClient = factory.Client;
            await usersClient.PostAsync(new Web.Api.User
            {
                Id = 42,
                FirstName = "John",
                LastName = "Smith"
            });
            HttpClient client = factory.CreateClient();

            var values = new Dictionary<string?, string?>
            {
                { nameof(UserViewModel.Id), "42" }
            };
            using FormUrlEncodedContent content = new(values);
            HttpResponseMessage response = await client.PostAsync("/users/delete", content);

            response.EnsureSuccessStatusCode();
            Assert.AreEqual(1, usersClient.DeleteInvocationCount);
            Assert.IsNull(await usersClient.GetAsync(42));
        }
    }
}
