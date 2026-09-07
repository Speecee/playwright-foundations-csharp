using Microsoft.Playwright;
using Bogus;
using System.Text.Encodings.Web;
using System.Security.Principal;

namespace CarvedRock.End2End
{

    
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class AdminTests : PageTest
    {

        private const string _authenticationStateFilename = "authState.json";
        private IPage _page = null!;
        internal string _baseUrl = null!;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _baseUrl = Utilities.GetBaseUrl();
        }

        [SetUp]
        public async Task Setup()
        {
            if (!File.Exists(_authenticationStateFilename))
            {
                await LoginAsAdmin();
            }

            var browserContext = await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                StorageStatePath = _authenticationStateFilename
            });

            _page = await browserContext.NewPageAsync();
        }


        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            File.Delete(_authenticationStateFilename);
        }


        [Test]
        public async Task AdminIsAvailableForBob()
        {
            //await LoginAsAdmin();
            await _page.GotoAsync(_baseUrl);
            await _page.GetByRole(AriaRole.Link, new() { Name = "Admin" }).ClickAsync();
            await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "Create New" })).ToBeVisibleAsync();

        }
        [Test]
        public async Task ValidationErrosAppearOnEmptySubmission()
        {
            //await LoginAsAdmin();
            await _page.GotoAsync(_baseUrl);
            await _page.GetByRole(AriaRole.Link, new() { Name = "Admin" }).ClickAsync();
            

            await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "Create New" })).ToBeVisibleAsync();
            await _page.GetByRole(AriaRole.Link, new() { Name = "Create New" }).ClickAsync();
          
            await Expect(_page.GetByRole(AriaRole.Button, new() { Name = "Create" })).ToBeVisibleAsync();
            await _page.GetByRole(AriaRole.Button, new() { Name = "Create" }).ClickAsync();

            await Expect(_page.Locator("form")).ToContainTextAsync("The Name field is required.");
            await Expect(_page.Locator("form")).ToContainTextAsync("The ImgUrl field is required.");
        }

        [Test]
        public async Task CanSupportNewProductSuccessfully()
        {
            //await LoginAsAdmin();
            await _page.GotoAsync(_baseUrl);
            await _page.GetByRole(AriaRole.Link, new() { Name = "Admin" }).ClickAsync();

            await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "Create New" })).ToBeVisibleAsync();
            await _page.GetByRole(AriaRole.Link, new() { Name = "Create New" }).ClickAsync();

            //await _page.ReloadAsync();

            var Categories = new List<string> { "kayak", "equip", "boots" };
            var ProductFaker = new Faker<Product>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Price, f => Convert.ToDecimal(f.Commerce.Price(20,150)))
                .RuleFor(p => p.Description, f => f.Lorem.Sentence())
                .RuleFor(p => p.Category, f => f.PickRandom(Categories))
                .RuleFor(p => p.ImgUrl, f => f.Image.PicsumUrl());

            var productObject = ProductFaker.Generate();

            await _page.GetByRole(AriaRole.Textbox, new() { Name = "Name" }).FillAsync(productObject.Name!);
            await _page.GetByRole(AriaRole.Textbox, new() { Name = "Price" }).FillAsync(productObject.Price.ToString()!); 
            await _page.GetByRole(AriaRole.Textbox, new() { Name = "Description" }).FillAsync(productObject.Description!);
            await _page.GetByRole(AriaRole.Textbox, new() { Name = "Category" }).FillAsync(productObject.Category!);
            await _page.GetByRole(AriaRole.Textbox, new() { Name = "ImgUrl" }).FillAsync(productObject.ImgUrl!);

            await _page.GetByRole(AriaRole.Button, new() { Name = "Create" }).ClickAsync();


            var NewProduct = _page.GetByTestId(productObject.Name!);
            var ProductRow = NewProduct.Locator("td", new() { HasTextString = productObject.Name });

            try
            {
                await Expect(ProductRow).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = 50_000
                });
                Console.WriteLine($"Yeah! New Product: {productObject.Name}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                await _page.ScreenshotAsync(new() { Path = "ErrorScreenshot.png" });
                throw;
            }


            //var NewProduct = table.Locator("tr", new() { HasTextString = productObject.Name });
            //await NewProduct.ScrollIntoViewIfNeededAsync();
            //new() { WaitUntil = WaitUntilState.Load });
            //await _page.GetByRole(AriaRole.Link, new() { Name = "Admin" }).ClickAsync();
            //await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "Create New" })).ToBeVisibleAsync();
            ////await _page.ReloadAsync(new() { WaitUntil = WaitUntilState.Load });
            //await Page.ScreenshotAsync(new() { Path = "CreateProductPage-screenshot.png" });
            //await Expect(_page.Locator("tbody")).ToContainTextAsync(productObject.Name!, new LocatorAssertionsToContainTextOptions
            //{
            //    Timeout = 50_000
            //});
            //await Expect(_page.Locator("tbody")).ToContainTextAsync(productObject.Description!);

        }

        [Test]
        public async Task CanDeleteProduct()
        {

            await _page.GotoAsync(_baseUrl);

            await _page.GetByRole(AriaRole.Link, new() { Name = "Admin" }).ClickAsync();

            void Page_Dialog_EventHandler(object? sender, IDialog dialog)
            {
                Console.WriteLine($"Dialog message: {dialog.Message}");
                dialog.DismissAsync();
                _page.Dialog -= Page_Dialog_EventHandler;
            }
            _page.Dialog += Page_Dialog_EventHandler;

            var deleteLink = _page.GetByTestId("Glide").GetByRole(AriaRole.Link, new() { Name = "Delete" });
            await deleteLink.ScreenshotAsync(new() { Path = "deleteLink.png" });
            await deleteLink.ClickAsync();

            await Expect(_page.GetByLabel("Confirm Delete")).ToContainTextAsync("Are you sure you want to delete the product \"Glide\" (ID: 6)?");

            await Expect(_page.GetByLabel("Confirm Delete").GetByText("Delete", new() { Exact = true })).ToBeVisibleAsync();
            await _page.GetByLabel("Confirm Delete").GetByText("Delete", new() { Exact = true }).ClickAsync();

            await Expect(_page.GetByRole(AriaRole.Heading)).ToContainTextAsync("Congratulations!");

        }


        private async Task LoginAsAdmin()
        {
            await Page.GotoAsync(_baseUrl);
            await Page.GetByRole(AriaRole.Link, new() { Name = "Sign in" }).ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync("bob");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).PressAsync("Tab");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("bob");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();

            

            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Admin" })).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
            {
                Timeout = 10_000
            });


            await Page.Context.StorageStateAsync(new() { Path = _authenticationStateFilename });

            Console.WriteLine("Logged in as admin user 'bob'.");
        }


    }
}
