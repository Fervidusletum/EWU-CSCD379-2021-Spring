using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSanta.Data;
using SecretSanta.Data.Tests;

namespace SecretSanta.Business.Tests
{
    [TestClass]
    public class GiftRepositoryTests : TestableDbContext
    {

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Create_NullItem_ThrowsArgumentException()
        {
            using DbContext dbContext = new(ContextOptions);
            Init(dbContext);
            GiftRepository sut = new(dbContext);

            sut.Create(null!);
        }

        [TestMethod]
        public void Create_WithItem_CanGetItem()
        {
            using DbContext dbContext = new(ContextOptions);
            Init(dbContext);
            GiftRepository sut = new(dbContext);
            Gift gift = new()
            {
                Id = 42,
                Title = "test",
                Receiver = new(),
                Priority = 1
            };

            Gift createdGift = sut.Create(gift);

            Gift? retrievedGift = sut.GetItem(createdGift.Id);
            Assert.AreEqual(gift, retrievedGift);
        }

        [TestMethod]
        public void GetItem_WithBadId_ReturnsNull()
        {
            using DbContext dbContext = new(ContextOptions);
            Init(dbContext);
            GiftRepository sut = new(dbContext);

            Gift? gift = sut.GetItem(-1);

            Assert.IsNull(gift);
        }

        [TestMethod]
        public void GetItem_WithValidId_ReturnsGift()
        {
            using DbContext dbContext = new(ContextOptions);
            Init(dbContext);
            GiftRepository sut = new(dbContext);
            sut.Create(new()
            {
                Id = 42,
                Title = "test",
                Receiver = new(),
                Priority = 1
            });

            Gift? gift = sut.GetItem(42);

            Assert.AreEqual(42, gift?.Id);
            Assert.AreEqual("test", gift!.Title);
            Assert.AreEqual(1, gift.Priority);
        }

        [TestMethod]
        public void List_WithGifts_ReturnsAllGift()
        {
            using DbContext dbContext = new(ContextOptions);
            Init(dbContext);
            GiftRepository sut = new(dbContext);

            int expected = dbContext.Gifts.Count() + 1;
            sut.Create(new()
            {
                Id = 42,
                Title = "test",
                Receiver = new(),
                Priority = 1
            });

            ICollection<Gift> gifts = sut.List();

            Assert.AreEqual(expected, gifts.Count);
            foreach (var mockGift in dbContext.Gifts)
            {
                Assert.IsNotNull(gifts.SingleOrDefault(x => x.Title == mockGift.Title && x.Priority == mockGift.Priority));
            }
        }

        [TestMethod]
        [DataRow(-1, false)]
        [DataRow(42, true)]
        public void Remove_WithInvalidId_ReturnsTrue(int id, bool expected)
        {
            using DbContext dbContext = new(ContextOptions);
            Init(dbContext);
            GiftRepository sut = new(dbContext);
            sut.Create(new()
            {
                Id = 42,
                Title = "test",
                Receiver = new(),
                Priority = 1
            });

            Assert.AreEqual(expected, sut.Remove(id));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Save_NullItem_ThrowsArgumentException()
        {
            using DbContext dbContext = new(ContextOptions);
            Init(dbContext);
            GiftRepository sut = new(dbContext);

            sut.Save(null!);
        }

        [TestMethod]
        public void Save_WithValidItem_SavesItem()
        {
            using DbContext dbContext = new(ContextOptions);
            Init(dbContext);
            GiftRepository sut = new(dbContext);

            Gift fake = new Gift() { Id = 42, Title = "test", Receiver = new(), Priority = 1};
            sut.Create(fake);

            string expected = "updated";
            fake.Title = expected;

            sut.Save(fake);

            Assert.AreEqual(expected, sut.GetItem(42)?.Title);
        }
    }
}
