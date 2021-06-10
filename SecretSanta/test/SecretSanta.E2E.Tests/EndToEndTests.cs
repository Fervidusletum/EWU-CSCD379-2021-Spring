using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaywrightSharp;

namespace SecretSanta.E2E.Tests
{
    [TestClass]
    public class EndToEndTests
    {
        private static WebHostServerFixture<Web.Startup, Api.Startup> _Server;
        [ClassInitialize]
        public static void InitializeClass(TestContext testContext)
        {
            _Server = new();
        }

        [TestMethod]
        public async Task LaunchHomepage()
        {
            var localhost = _Server.WebRootUri.AbsoluteUri.Replace("127.0.0.1", "localhost");
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
            var context = await browser.NewContextAsync(ignoreHTTPSErrors: true);
            var page = await context.NewPageAsync();

            var response = await page.GoToAsync(localhost);
            Assert.IsTrue(response.Ok);

            var headerContent = await page.GetTextContentAsync("body > header > div > a");
            Assert.AreEqual("Secret Santa", headerContent);
        }

        [TestMethod]
        public async Task VerifyAllNavigationLinksInHeaderWork()
        {
            var localhost = _Server.WebRootUri.AbsoluteUri.Replace("127.0.0.1", "localhost");
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
            var context = await browser.NewContextAsync(ignoreHTTPSErrors: true);
            var page = await context.NewPageAsync();

            var response = await page.GoToAsync(localhost);
            Assert.IsTrue(response.Ok);

            await page.ClickAsync("text=Users");
            var button = await page.WaitForSelectorAsync("a:has-text('Create User')");
            Assert.IsNotNull(button);

            await page.ClickAsync("text=Groups");
            button = await page.WaitForSelectorAsync("a:has-text('Create Group')");
            Assert.IsNotNull(button);
        }

        [TestMethod]
        [Ignore]
        public async Task CreateGift()
        {
            var localhost = _Server.WebRootUri.AbsoluteUri.Replace("127.0.0.1", "localhost");
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            var page = await browser.NewPageAsync();
            var response = await page.GoToAsync(localhost);

            Assert.IsTrue(response.Ok);

            await page.ClickAsync("text=Gifts");

            var gifts = await page.QuerySelectorAllAsync("body > section > section > section");
            Assert.AreEqual(4, gifts.Count());

            await page.ClickAsync("text=Create");

            await page.TypeAsync("input#Title", "Simple Gift");
            await page.TypeAsync("input#Description", "Just a simple description");
            await page.TypeAsync("input#Url", "https://www.somegift.com");
            await page.TypeAsync("input#Priority", "3");
            await page.SelectOptionAsync("select#UserId", "2");

            await page.ClickAsync("text=Create");

            gifts = await page.QuerySelectorAllAsync("body > section > section > section");
            Assert.AreEqual(5, gifts.Count());
        }

        [TestMethod]
        [Ignore]
        public async Task ModifyLastGift()
        {
            var localhost = _Server.WebRootUri.AbsoluteUri.Replace("127.0.0.1", "localhost");
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new LaunchOptions
            {
                Headless = false
            });

            var page = await browser.NewPageAsync();
            var response = await page.GoToAsync(localhost);

            Assert.IsTrue(response.Ok);

            await page.ClickAsync("text=Gifts");

            var sectionText = await page.GetTextContentAsync("body > section > section > section:last-child > a > section > div");
            Assert.AreEqual("Simple Gift", sectionText);

            await page.ClickAsync("body > section > section > section:last-child");

            await page.ClickAsync("input#Title", clickCount:3); // Select all text in the text box
            await page.TypeAsync("input#Title", "Updated Gift");
            await page.ClickAsync("text=Update");

            sectionText = await page.GetTextContentAsync("body > section > section > section:last-child > a > section > div");
            Assert.AreEqual("Updated Gift", sectionText);
        }

        [TestMethod]
        [Ignore]
        public async Task DeleteLastGift()
        {
            var localhost = _Server.WebRootUri.AbsoluteUri.Replace("127.0.0.1", "localhost");
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            var page = await browser.NewPageAsync();
            var response = await page.GoToAsync(localhost);

            Assert.IsTrue(response.Ok);

            await page.ClickAsync("text=Gifts");

            var gifts = await page.QuerySelectorAllAsync("body > section > section > section");
            Assert.AreEqual(5, gifts.Count());

            page.Dialog += (_, args) => args.Dialog.AcceptAsync();

            await page.ClickAsync("body > section > section > section:last-child > a > section > form > button");
            gifts = await page.QuerySelectorAllAsync("body > section > section > section");
            Assert.AreEqual(4, gifts.Count());
        }
    }
}
