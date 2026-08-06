using System;
using Microsoft.Playwright;
using System.Text;
using System.Text.Json;

namespace CarvedRock.End2End
{
    public class ApiTests: PlaywrightTest
    {

        private IAPIRequestContext _request = null!;

        private readonly JsonSerializerOptions _jsonOptions = new ()
        {
            PropertyNameCaseInsensitive = true
        };

        [SetUp]
        public async Task Setup()
        {
            _request = await Playwright.APIRequest.NewContextAsync(new()
            {
                BaseURL = "https://localhost:7213",
                IgnoreHTTPSErrors = true
            });
        }

        [Test]
        public async Task GetProductsReturnsInitialProducts() 
        {
            var productResponse = await _request.GetAsync("/Product?category=all");
            await Expect(productResponse).ToBeOKAsync();


            var productJson = await productResponse.TextAsync();
            var products = JsonSerializer.Deserialize<List<Product>>(productJson, _jsonOptions);

            var expectedNames = new[] { "Trailblazer", "Coastliner", "Woodsman", "Billy", "Sherpa"
                , "Glide"  };

            foreach (var expectedName in expectedNames)
            {
                var product = products!.FirstOrDefault(p => p.Name == expectedName);
                Console.Write(product!.Name!.ToString() + " : ");
                Assert.That(product, Is.Not.Null);
            }
        }
    }
}
