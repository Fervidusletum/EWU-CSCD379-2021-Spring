using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSanta.Api.Controllers;
using SecretSanta.Api.Tests.Business;

namespace SecretSanta.Api.Tests.Controllers
{
    [TestClass]
    public class UserControllerTests
    {
        [TestMethod]
        public void Constructor_WithNullRepository_ThrowException()
        {
            var ex = Assert.ThrowsException<ArgumentNullException>(() => new UsersController(null!));
            Assert.AreEqual("repository", ex.ParamName);
        }

        [TestMethod]
        public async Task GetAll_WithUsers_RetrievesUsers()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableUserRepository repository = factory.UserRepository;
            Data.User user = new()
            {
                Id = 42,
                FirstName = "John",
                LastName = "Smith"
            };
            repository.Create(user);

            HttpClient client = factory.CreateClient();
            
            //Act
            List<Dto.User>? users = await client.GetFromJsonAsync<List<Dto.User>>("/api/users");

            //Assert
            Assert.AreEqual(1, users!.Count);
            Assert.AreEqual(42, users[0].Id);
            Assert.AreEqual("John", users[0].FirstName);
            Assert.AreEqual("Smith", users[0].LastName);
        }

        [TestMethod]
        public async Task GetById_WithUser_RetrievesUser()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableUserRepository repository = factory.UserRepository;
            Data.User user = new()
            {
                Id = 42,
                FirstName = "John",
                LastName = "Smith"
            };
            repository.Create(user);

            HttpClient client = factory.CreateClient();

            //Act
            Dto.User? retrievedUser = await client.GetFromJsonAsync<Dto.User>("/api/users/42");

            //Assert
            Assert.AreEqual(42, retrievedUser!.Id);
            Assert.AreEqual("John", retrievedUser.FirstName);
            Assert.AreEqual("Smith", retrievedUser.LastName);
        }

        [TestMethod]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableUserRepository repository = factory.UserRepository;
            Data.User user = new()
            {
                Id = 42,
                FirstName = "John",
                LastName = "Smith"
            };
            repository.Create(user);

            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.GetAsync("/api/users/41");

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task Delete_WithValidId_RemovesItem()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableUserRepository repository = factory.UserRepository;
            Data.User user = new()
            {
                Id = 42,
                FirstName = "John",
                LastName = "Smith"
            };
            repository.Create(user);

            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.DeleteAsync("/api/users/42");

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(0, repository.List().Count);
        }

        [TestMethod]
        public async Task Delete_WithInvalidId_ReturnsNotFound()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableUserRepository repository = factory.UserRepository;
            Data.User user = new()
            {
                Id = 42,
                FirstName = "John",
                LastName = "Smith"
            };
            repository.Create(user);

            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.DeleteAsync("/api/users/41");

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.IsTrue(repository.List().Any());
        }

        [TestMethod]
        public async Task Create_ValidData_CreatesUser()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableUserRepository repository = factory.UserRepository;

            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.PostAsJsonAsync("/api/users/", new Dto.User
            {
                Id = 42,
                FirstName = "John",
                LastName = "Smith"
            });

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var createdUser = repository.GetItem(42);
            Assert.AreEqual(42, createdUser.Id);
            Assert.AreEqual("John", createdUser.FirstName);
            Assert.AreEqual("Smith", createdUser.LastName);
        }

        [TestMethod]
        public async Task Update_ValidData_UpdatesUser()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableUserRepository repository = factory.UserRepository;
            Data.User user = new()
            {
                Id = 42,
                FirstName = "John",
                LastName = "Smith"
            };
            repository.Create(user);
            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.PutAsJsonAsync("/api/users/42", new Dto.UpdateUser
            {
                FirstName = "Jane",
                LastName = "Doe"
            });

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var createdUser = repository.GetItem(42);
            Assert.AreEqual(42, createdUser.Id);
            Assert.AreEqual("Jane", createdUser.FirstName);
            Assert.AreEqual("Doe", createdUser.LastName);
        }

        [TestMethod]
        public async Task Update_InvalidUserId_ReturnsNotFound()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableUserRepository repository = factory.UserRepository;
            Data.User user = new()
            {
                Id = 42,
                FirstName = "John",
                LastName = "Smith"
            };
            repository.Create(user);
            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.PutAsJsonAsync("/api/users/41", new Dto.UpdateUser
            {
                FirstName = "Jane",
                LastName = "Doe"
            });

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            var createdUser = repository.GetItem(42);
            Assert.AreEqual(42, createdUser.Id);
            Assert.AreEqual("John", createdUser.FirstName);
            Assert.AreEqual("Smith", createdUser.LastName);
        }
    }
}
