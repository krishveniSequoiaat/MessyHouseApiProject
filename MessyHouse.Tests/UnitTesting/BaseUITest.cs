
using Microsoft.Playwright;

namespace MessyHouse.Tests.UnitTesting
{
    // Base class for UI tests
    // This class can be extended to include common setup or utilities for UI testing

    public class BaseUITest : IAsyncLifetime
    {
        protected IPlaywright Playwright { get; private set; }
        protected IBrowser Browser { get; private set; }
        protected IBrowserContext Context { get; private set; }
        protected IPage Page { get; private set; }
        // Common setup or utilities for UI tests can be added here
        public async Task DisposeAsync()
        {
            await Browser.CloseAsync();
            Playwright.Dispose();
        }

        public async Task InitializeAsync()
        {
            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 50,
                Args = new[]
                        {
                            "--use-fake-ui-for-media-stream",
                            "--use-fake-device-for-media-stream",
                            "--no-user-gesture-required"
                        }
            });

            Context = await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                Permissions = new[] { "camera" },   // Grant camera permission
                IgnoreHTTPSErrors = true
            });
            Page = await Context.NewPageAsync();
        }
    }
}