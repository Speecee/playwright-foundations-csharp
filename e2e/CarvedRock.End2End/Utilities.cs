using Microsoft.Playwright;

namespace CarvedRock.End2End
{
    internal class Utilities
    {
        public static string GetBaseUrl()
        {
            return TestContext.Parameters["BaseUrl"]!;
        }

        public static string GetApiUrl()
        {
            return TestContext.Parameters["ApiUrl"]!; 
        }
    }
}
