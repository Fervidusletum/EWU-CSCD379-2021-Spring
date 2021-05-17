using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using PlaywrightSharp;
using System.Linq;

namespace SecretSanta.E2E
{
    [TestClass]
    public class EndToEndTests
    {
        private static WebHostServerFixture<Web.Startup, SecretSanta.Api.Startup> Server;

        [ClassInitialize]
        public static void InitializeClass(TestContext testContext)
        {
            Server = new();
        }

        [TestMethod]
        public async Task LaunchHomepage()
        {
            Assert.Fail();
        }

        [TestMethod]
        public async Task Homepage_NavBannerContainsSecretSanta()
        {
            Assert.Fail();
        }

        [TestMethod]
        public async Task NavTo_GiftsIndex()
        {
            Assert.Fail();
        }

        [TestMethod]
        public async Task NavTo_GroupsIndex()
        {
            Assert.Fail();
        }

        [TestMethod]
        public async Task NavTo_UsersIndex()
        {
            Assert.Fail();
        }

        [TestMethod]
        public async Task Gifts_Create()
        {
            Assert.Fail();
        }

        [TestMethod]
        public async Task Gifts_ModifyLast()
        {
            Assert.Fail();
        }

        [TestMethod]
        public async Task Gifts_DeleteLast()
        {
            Assert.Fail();
        }
    }
}
