using Microsoft.Playwright;

namespace CarvedRock.End2End
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class Tests : PageTest
    {
        internal string _baseUrl = null!;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _baseUrl = Utilities.GetBaseUrl();
        }

        [SetUp]
        public void Setup()
        {
            TestContext.Out.WriteLine($"Useing base url: {_baseUrl}");
        }



        [Test]
        public async Task HomePageHasCorrectContent()
        {
            await Page.GotoAsync(_baseUrl);

            // Expect a title "to contain" a substring.
            await Expect(Page).ToHaveTitleAsync("Carved Rock Fitness");

            await Page.ScreenshotAsync(new() { Path = "HomePage-screenshot.png" });

            var bannerTextLocator = Page.GetByText("GET A GRIP");
            await bannerTextLocator.ScreenshotAsync(new() { Path = "banner-screenshot.png" });
            await bannerTextLocator.HighlightAsync();
            await bannerTextLocator.ScreenshotAsync(new() { Path = "banner-highlighted.png" });

            
            await Page.GetByRole(AriaRole.Heading, new() { Name = "GET A GRIP" }).ClickAsync();
            await Page.GetByRole(AriaRole.Heading, new() { Name = "% OFF" }).ClickAsync();
            await Page.GetByText("GET A GRIP 20% OFF THROUGHOUT").ClickAsync();


        }

        [Test]
        public async Task CanAddItemsToCartOnFootwearPage()
        {
            await Page.GotoAsync(_baseUrl);
            await Page.GetByRole(AriaRole.Link, new() { Name = "Footwear" }).ClickAsync();
            await Expect(Page.GetByRole(AriaRole.Img, new() { Name = "Trailblazer" })).ToBeVisibleAsync();
            await Expect(Page.GetByAltText("Trailblazer")).ToBeVisibleAsync();


            var addBtn = Page.Locator("#add-btn-1");
            await Expect(addBtn).ToBeVisibleAsync();
            await addBtn.ClickAsync();

            await Expect(Page.Locator("#carvedrockcart")).ToContainTextAsync("Cart (1)");

        }

        [TestCaseSource(nameof(Users))]
        public async Task AddItemsToCartAndVerifyContents(User user)
        {
            await Page.GotoAsync(_baseUrl + "/Listing?cat=boots");
            await Page.GetByTestId("Trailblazer").GetByRole(AriaRole.Button, new() { Name = "Add to Cart" }).ClickAsync();

            //await Page.GetByTestId("Coastliner").GetByRole(AriaRole.Button, new() { Name = "Add to Cart" }).ClickAsync();
            //await Page.GetByTestId("Coastliner").GetByRole(AriaRole.Button, new() { Name = "Add to Cart" }).ClickAsync();
            var addCoastLiner = Page.Locator("#add-btn-2");
            await addCoastLiner.ClickAsync();
            await addCoastLiner.ClickAsync();


            await Page.GetByRole(AriaRole.Link, new() { NameString = "Cart" }).ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync(user.Username);
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).PressAsync("Tab");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync(user.Password);
            await Page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();

            //await Expect(Page.Locator("tbody")).ToContainTextAsync("1");


            var table = Page.Locator("table");
            var tableRows = table.Locator("tr");
            await Expect(tableRows).ToHaveCountAsync(5);

            var CoastlinerRow = table.Locator("tr", new() { HasTextString = "Coastliner" });
            await Expect(CoastlinerRow).ToBeVisibleAsync();
            await Expect(CoastlinerRow).ToHaveCountAsync(1);
            var TrailblazerRow = table.Locator("tr", new() { HasTextString = "Trailblazer" });
            await Expect(TrailblazerRow).ToBeVisibleAsync();
            await Expect(TrailblazerRow).ToHaveCountAsync(1);



            var cellValue = CoastlinerRow.Locator("td").Nth(3);
            await Expect(cellValue).ToContainTextAsync("2");

            var grandTotal = await Page.Locator("#grand-total").TextContentAsync();
            Console.WriteLine(grandTotal!.ToString());
            //await Expect(Page.Locator("#grand-total")).ToContainTextAsync("$119.98");
        }

        [Test]
        public async Task DelayedContentShowsUp()
        {
            await Page.GotoAsync(_baseUrl);
            await Page.GetByRole(AriaRole.Link, new() { Name = "Footwear" }).ClickAsync();
            await Expect(Page.Locator("#content-with-delay")).ToContainTextAsync("This content was delayed by 7000 milliseconds",
               new LocatorAssertionsToContainTextOptions
               {
                   Timeout = 15_000
               });

        }

        public static User[] Users =
          [
              new("bob", "bob" ),
              new("alice", "alice")
          ];

        public record User(string Username, string Password);

    }
}
