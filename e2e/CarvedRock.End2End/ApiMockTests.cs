using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Playwright;
using Microsoft.VisualBasic;

namespace CarvedRock.End2End
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    internal class ApiMockTests: PageTest
    {
        [SetUp]
        public async Task Setup()
        {
            await Context.Tracing.StartAsync(new()
            {
                Title = "ApiMockTests",
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }

        [Test]
        public async Task MockedItemsOnFootwearPage()
        {
            await Page.GotoAsync("https://localhost:7224");

            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
            string fileLocation = Path.Combine(projectRoot, "boots-mock.json");

            await Page.RouteAsync("*/**/Product?category=boots", async route =>
            {
                //var json = new[] { new { name = "Strawberry", id = 21 } };
                await route.FulfillAsync(new()
                {
                    Path = fileLocation
                });
            });

           
            await Page.GetByRole(AriaRole.Link, new() { Name = "Kayaks" }).ClickAsync();
            await Expect(Page.GetByAltText("Glide")).ToBeVisibleAsync();
        }

        [TearDown]
        public async Task StopTracing() 
        {
            await Context.Tracing.StopAsync(new()
            {
                Path = Path.Combine(Environment.CurrentDirectory,
                "playwright-traces",
                "api-mock-traces.zip")
            });
        }

    }
}
