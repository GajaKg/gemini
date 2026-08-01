
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumUndetectedChromeDriver;

namespace gemini.Services.HtmlProviders
{
    public class SeleniumProvider : ISeleniumProvider
    {
        private readonly ILogger<SeleniumProvider> _logger;

        public SeleniumProvider(ILogger<SeleniumProvider> logger)
        {
            _logger = logger;
        }

        public async Task<HtmlDocument?> GetHtml(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                var driver = await CreateDriver();

                await driver.Navigate().GoToUrlAsync(url);
                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(2));

                if (driver.ExecuteScript("return document.documentElement.outerHTML;") is not string html) return null;

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                return doc;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "❌ Failed request");
                return null;
            }

        }


        private async Task<UndetectedChromeDriver> CreateDriver()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--headless=new");

            return UndetectedChromeDriver.Create(
                options: chromeOptions,
                driverExecutablePath: await new ChromeDriverInstaller().Auto()
            );
        }
    }
}