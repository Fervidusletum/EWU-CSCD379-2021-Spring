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
    public class GiftsControllerTests
    {
        [TestMethod]
        public void Constructor_WithNullRepository_ThrowException()
        {
            var ex = Assert.ThrowsException<ArgumentNullException>(() => new GiftsController(null!));
            Assert.AreEqual("repository", ex.ParamName);
        }

        [TestMethod]
        public async Task GetAll_WithGifts_RetrievesGifts()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableGiftRepository repository = factory.GiftRepository;
            Data.Gift gift = new()
            {
                Id = 42,
                Title = "test",
                Priority = 1,
                ReceiverId = 42
            };
            repository.Create(gift);

            HttpClient client = factory.CreateClient();

            //Act
            List<Dto.Gift>? gifts = await client.GetFromJsonAsync<List<Dto.Gift>>($"/api/gifts/byuser/{gift.ReceiverId}");

            //Assert
            Assert.AreEqual(1, gifts!.Count);
            Assert.AreEqual(42, gifts[0].Id);
            Assert.AreEqual("test", gifts[0].Title);
            Assert.AreEqual(1, gifts[0].Priority);
        }

        [TestMethod]
        public async Task GetById_WithGift_RetrievesGift()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableGiftRepository repository = factory.GiftRepository;
            Data.Gift gift = new()
            {
                Id = 42,
                Title = "test",
                Priority = 1
            };
            repository.Create(gift);

            HttpClient client = factory.CreateClient();

            //Act
            Dto.Gift? retrievedGift = await client.GetFromJsonAsync<Dto.Gift>("/api/gifts/42");

            //Assert
            Assert.AreEqual(42, retrievedGift!.Id);
            Assert.AreEqual("test", retrievedGift!.Title);
            Assert.AreEqual(1, retrievedGift!.Priority);
        }

        [TestMethod]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableGiftRepository repository = factory.GiftRepository;
            Data.Gift gift = new()
            {
                Id = 42,
                Title = "test",
                Priority = 1
            };
            repository.Create(gift);

            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.GetAsync("/api/gifts/41");

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task Delete_WithValidId_RemovesItem()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableGiftRepository repository = factory.GiftRepository;
            Data.Gift gift = new()
            {
                Id = 42,
                Title = "test",
                Priority = 1
            };
            repository.Create(gift);

            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.DeleteAsync("/api/gifts/42");

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(0, repository.List().Count);
        }

        [TestMethod]
        public async Task Delete_WithInvalidId_ReturnsNotFound()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableGiftRepository repository = factory.GiftRepository;
            Data.Gift gift = new()
            {
                Id = 42,
                Title = "test",
                Priority = 1
            };
            repository.Create(gift);

            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.DeleteAsync("/api/gifts/41");

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.IsTrue(repository.List().Any());
        }

        [TestMethod]
        public async Task Create_ValidData_CreatesGift()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableGiftRepository repository = factory.GiftRepository;

            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.PostAsJsonAsync("/api/gifts/", new Dto.Gift
            {
                Id = 42,
                Title = "test",
                Priority = 1
            });

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var createdGift = repository.GetItem(42);
            Assert.AreEqual(42, createdGift.Id);
            Assert.AreEqual("test", createdGift.Title);
            Assert.AreEqual(1, createdGift.Priority);
        }

        [TestMethod]
        public async Task Update_ValidData_UpdatesGift()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableGiftRepository repository = factory.GiftRepository;
            Data.Gift gift = new()
            {
                Id = 42,
                Title = "test",
                Priority = 1,
                Description = "description",
                Url = "url"
            };
            repository.Create(gift);
            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.PutAsJsonAsync("/api/gifts/42", new Dto.UpdateGift
            {
                Title = "updated",
                Priority = 2
            });

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var createdGift = repository.GetItem(42);
            Assert.AreEqual(42, createdGift.Id);
            Assert.AreEqual("updated", createdGift.Title);
            Assert.AreEqual(2, createdGift.Priority);
            Assert.AreEqual("description", createdGift.Description);
            Assert.AreEqual("url", createdGift.Url);
        }

        [TestMethod]
        public async Task Update_InvalidGiftId_ReturnsNotFound()
        {
            //Arrange
            using WebApplicationFactory factory = new();
            TestableGiftRepository repository = factory.GiftRepository;
            Data.Gift gift = new()
            {
                Id = 42,
                Title = "test",
                Priority = 1
            };
            repository.Create(gift);
            HttpClient client = factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.PutAsJsonAsync("/api/gifts/41", new Dto.UpdateGift
            {
                Title = "updated",
                Priority = 2
            });

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            var createdGift = repository.GetItem(42);
            Assert.AreEqual(42, createdGift.Id);
            Assert.AreEqual("test", createdGift.Title);
            Assert.AreEqual(1, createdGift.Priority);
        }
    }
}
