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
using SecretSanta.Business;

namespace SecretSanta.Api.Tests.Controllers
{
    [TestClass]
    public class GroupsControllerTests
    {
        [TestMethod]
        public void Constructor_WithNullGroupRepository_ThrowException()
        {
            var ex = Assert.ThrowsException<ArgumentNullException>(() => new GroupsController(null!, new TestableUserRepository()));
            Assert.AreEqual("repository", ex.ParamName);
        }

        [TestMethod]
        public void Constructor_WithNullUserRepository_ThrowException()
        {
            var ex = Assert.ThrowsException<ArgumentNullException>(() => new GroupsController(new TestableGroupRepository(), null!));
            Assert.AreEqual("userRepository", ex.ParamName);
        }

        [TestMethod]
        public async Task GetAll_WithGroups_RetrievesGroups()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableGroupRepository repository = factory.GroupRepository;
            Data.Group group = new()
            {
                Id = 42,
                Name = "Group"
            };
            repository.Create(group);

            HttpClient client = factory.CreateClient();
            
            //Act
            List<Dto.Group>? groups = await client.GetFromJsonAsync<List<Dto.Group>>("/api/groups");

            //Assert
            Assert.AreEqual(1, groups!.Count);
            Assert.AreEqual(42, groups[0].Id);
            Assert.AreEqual("Group", groups[0].Name);
        }

        [TestMethod]
        public async Task GetById_WithGroup_RetrievesGroup()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableGroupRepository repository = factory.GroupRepository;
            Data.Group group = new()
            {
                Id = 42,
                Name = "Group",
            };
            repository.Create(group);

            HttpClient client = factory.CreateClient();

            //Act
            Dto.Group? retrievedGroup = await client.GetFromJsonAsync<Dto.Group>("/api/groups/42");

            //Assert
            Assert.AreEqual(42, retrievedGroup!.Id);
            Assert.AreEqual("Group", retrievedGroup.Name);
        }

        [TestMethod]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableGroupRepository repository = factory.GroupRepository;
            Data.Group group = new()
            {
                Id = 42,
                Name = "Group"
            };
            repository.Create(group);

            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.GetAsync("/api/groups/41");

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task Delete_WithValidId_RemovesItem()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableGroupRepository repository = factory.GroupRepository;
            Data.Group group = new()
            {
                Id = 42,
                Name = "Group"
            };
            repository.Create(group);

            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.DeleteAsync("/api/groups/42");

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(0, repository.List().Count);
        }

        [TestMethod]
        public async Task Delete_WithInvalidId_ReturnsNotFound()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableGroupRepository repository = factory.GroupRepository;
            Data.Group group = new()
            {
                Id = 42,
                Name = "Group"
            };
            repository.Create(group);

            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.DeleteAsync("/api/groups/41");

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.IsTrue(repository.List().Any());
        }

        [TestMethod]
        public async Task Create_ValidData_CreatesGroup()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableGroupRepository repository = factory.GroupRepository;

            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.PostAsJsonAsync("/api/groups/", new Dto.Group
            {
                Id = 42,
                Name = "Group"
            });

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var createdGroup = repository.GetItem(42);
            Assert.AreEqual(42, createdGroup?.Id);
            Assert.AreEqual("Group", createdGroup!.Name);
        }

        [TestMethod]
        public async Task Update_ValidData_UpdatesGroup()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableGroupRepository repository = factory.GroupRepository;
            Data.Group group = new()
            {
                Id = 42,
                Name = "Group"
            };
            repository.Create(group);
            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.PutAsJsonAsync("/api/groups/42", new Dto.UpdateGroup
            {
                Name = "Changed"
            });

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var createdGroup = repository.GetItem(42);
            Assert.AreEqual(42, createdGroup?.Id);
            Assert.AreEqual("Changed", createdGroup!.Name);
        }

        [TestMethod]
        public async Task Update_InvalidGroupId_ReturnsNotFound()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableGroupRepository repository = factory.GroupRepository;
            Data.Group group = new()
            {
                Id = 42,
                Name = "Group"
            };
            repository.Create(group);
            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.PutAsJsonAsync("/api/groups/41", new Dto.UpdateGroup
            {
                Name = "Changed"
            });

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            var createdGroup = repository.GetItem(42);
            Assert.AreEqual(42, createdGroup?.Id);
            Assert.AreEqual("Group", createdGroup!.Name);
        }

        [TestMethod]
        public async Task CreateAssignments_WithSuccess_ReturnsOk()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableGroupRepository repository = factory.GroupRepository;
            repository.AssignmentResult = AssignmentResult.Success();
            Data.Group group = new()
            {
                Id = 42,
                Name = "Group",
            };
            repository.Create(group);
            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.PutAsJsonAsync("/api/groups/42/assign", new Dto.UpdateGroup
            {
                Name = "Changed"
            });

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task CreateAssignments_WithError_ReturnsBadRequest()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableGroupRepository repository = factory.GroupRepository;
            repository.AssignmentResult = AssignmentResult.Error("Some error");
            Data.Group group = new()
            {
                Id = 42,
                Name = "Group",
            };
            repository.Create(group);
            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.PutAsJsonAsync("/api/groups/42/assign", new Dto.UpdateGroup
            {
                Name = "Changed"
            });

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
