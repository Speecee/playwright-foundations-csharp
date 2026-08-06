using Microsoft.Playwright;

namespace CarvedRock.End2End
{
    internal class Utilities
    {
        public static string GetBaseUrl()
        {
            return TestContext.Parameters.Get("BaseUrl", "https://localhost:7224");
        }

        public static string GetApiUrl()
        {
            return TestContext.Parameters.Get("BaseUrl", "https://localhost:7213");
        }
    }
}
