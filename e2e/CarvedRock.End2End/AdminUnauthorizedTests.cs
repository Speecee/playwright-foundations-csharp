using Microsoft.Playwright;

namespace CarvedRock.End2End
{

    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class AdminUnauthorizedTests : PageTest
    {
        [Test]
        public async Task AdminIsGuardedAndRedirectsAnonymousUsersToLogin()
        {
           await Page.GotoAsync("https://localhost:7224/Admin");
           await Expect(Page).ToHaveURLAsync(new Regex("https://demo.duendesoftware.com/Account/Login.*"));
        }

        [Test]
        public async Task AdminIsNotAvailableForAlice()
        {
            await Page.GotoAsync("https://localhost:7224/");
            await Page.GetByRole(AriaRole.Link, new() { Name = "Sign in" }).ClickAsync();
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync("alice");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).PressAsync("Tab");
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("alice");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();

            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Admin" })).ToBeHiddenAsync();
            await Page.GotoAsync("https://localhost:7224/Admin");
            await Expect(Page).ToHaveURLAsync("https://localhost:7224/AccessDenied?ReturnUrl=%2FAdmin");

            await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Access Denied" })).ToBeVisibleAsync();

        }
    }
}
