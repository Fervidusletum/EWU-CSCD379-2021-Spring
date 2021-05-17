using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using PlaywrightSharp;
using PlaywrightSharp.Chromium;
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
        public async Task Homepage_NavBannerContainsSecretSanta()
        {
            string? localhost = Server.WebRootUri.AbsoluteUri.Replace("127.0.0.1", "localhost");
            using IPlaywright? playwright = await Playwright.CreateAsync();
            await using IChromiumBrowser? browser = await playwright.Chromium.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            IPage? page = await browser.NewPageAsync();
            IResponse? response = await page.GoToAsync(localhost);

            Assert.IsTrue(response.Ok, "Could not reach home page.");

            string? headerContent = await page.GetTextContentAsync("body > header > div > a");
            Assert.AreEqual("Secret Santa", headerContent, "Secret Santa not found in nav banner.");
        }

        [DataTestMethod]
        [DataRow("Users", 1)]
        [DataRow("Groups", 2)]
        [DataRow("Gifts", 3)]
        public async Task NavTo_IndexPages(string indexpage, int nc)
        {
            string? localhost = Server.WebRootUri.AbsoluteUri.Replace("127.0.0.1", "localhost");
            using IPlaywright? playwright = await Playwright.CreateAsync();
            await using IChromiumBrowser? browser = await playwright.Chromium.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            IPage? page = await browser.NewPageAsync();
            IResponse? response = await page.GoToAsync(localhost);

            await page.ClickAsync($"text={indexpage}");

            Assert.IsTrue(page.Url.Contains(indexpage), $"Could not reach {indexpage} index.");

            string? headerContent = await page.GetTextContentAsync($"#headerNav > ul > li:nth-child({nc}) > a");
            Assert.AreEqual(indexpage, headerContent, $"{indexpage} not found in nav banner.");
        }

        [TestMethod]
        public async Task Gifts_Create()
        {
            string? localhost = Server.WebRootUri.AbsoluteUri.Replace("127.0.0.1", "localhost");
            using IPlaywright? playwright = await Playwright.CreateAsync();
            await using IChromiumBrowser? browser = await playwright.Chromium.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            IPage? page = await browser.NewPageAsync();
            IResponse? response = await page.GoToAsync(localhost);

            await page.ClickAsync("text=Gifts");

            int numGifts = (await page.QuerySelectorAllAsync("body > section > section > section")).Count();

            await page.ClickAsync("text=Create");
            await page.TypeAsync("input#Title", "Test Gift");
            await page.TypeAsync("input#Priority", "1");
            await page.SelectOptionAsync("select#UserId", "1");
            await page.ClickAsync("text=Create");

            int numGiftsAfterCreate = (await page.QuerySelectorAllAsync("body > section > section > section")).Count();
            Assert.AreEqual<int>(numGifts + 1, numGiftsAfterCreate, "Number of gifts did not increase by one after create.");
        }

        [TestMethod]
        public async Task Gifts_ModifyLast()
        {
            string? localhost = Server.WebRootUri.AbsoluteUri.Replace("127.0.0.1", "localhost");
            using IPlaywright? playwright = await Playwright.CreateAsync();
            await using IChromiumBrowser? browser = await playwright.Chromium.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            IPage? page = await browser.NewPageAsync();
            IResponse? response = await page.GoToAsync(localhost);

            await page.ClickAsync("text=Gifts");

            string? giftText = await page.GetTextContentAsync("body > section > section > section:last-child > a > section > div");
            string modifiedText = "Modified Gift";

            Assert.AreNotEqual<string>(giftText, modifiedText, "Test trying to update gift with the same text.");

            await page.ClickAsync("body > section > section > section:last-child > a");
            await page.PressAsync("input#Title", "Control+a");
            await page.TypeAsync("input#Title", modifiedText);
            await page.ClickAsync("text=Update");

            string? giftTextAfterModify = await page.GetTextContentAsync("body > section > section > section:last-child > a > section > div");
            Assert.AreEqual<string>(modifiedText, giftTextAfterModify, "Last gift does not match expected text.");
        }

        [TestMethod]
        public async Task Gifts_DeleteLast()
        {
            string? localhost = Server.WebRootUri.AbsoluteUri.Replace("127.0.0.1", "localhost");
            using IPlaywright? playwright = await Playwright.CreateAsync();
            await using IChromiumBrowser? browser = await playwright.Chromium.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            IPage? page = await browser.NewPageAsync();
            IResponse? response = await page.GoToAsync(localhost);

            await page.ClickAsync("text=Gifts");

            int numGifts = (await page.QuerySelectorAllAsync("body > section > section > section")).Count();

            page.Dialog += (_, args) => args.Dialog.AcceptAsync();
            await page.ClickAsync("body > section > section > section:last-child > a > section > form > button");

            int numGiftsAfterCreate = (await page.QuerySelectorAllAsync("body > section > section > section")).Count();
            Assert.AreEqual<int>(numGifts - 1, numGiftsAfterCreate, "Number of gifts did not decrease by one after deletion.");
        }
    }
}
