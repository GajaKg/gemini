
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumUndetectedChromeDriver;

namespace gemini.Services.HtmlProviders
{
    public class SeleniumProvider : ISeleniumProvider
    {
        private UndetectedChromeDriver? _undetectedChromeDriver;
        private readonly ILogger<SeleniumProvider> _logger;

        public SeleniumProvider(ILogger<SeleniumProvider> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Navigates to given url and downloading page html
        /// </summary>
        /// <param name="url">Page url</param>
        /// <param name="waitElements">
        /// If needed we can await for specific elements to appear
        /// By.CssSelector(".object_name")
        /// </param>
        /// <returns>HtmlDocument</returns>
        public async Task<HtmlDocument?> GetHtml(
            string url,
            By? waitElements,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var driver = await GetDriver();

                await driver.Navigate().GoToUrlAsync(url);

                // waiting for specific html element to appear on page
                if (waitElements is not null)
                {
                    WebDriverWait wait = new(driver, TimeSpan.FromSeconds(7));
                    wait.Until(d =>
                        d.FindElements(waitElements).Count > 0);
                }

                if (driver.ExecuteScript("return document.documentElement.outerHTML;") is not string html) return null;

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                return doc;
            }
            catch (WebDriverTimeoutException e)
            {
                _logger.LogWarning(
                    e,
                    "⏱️  Timeout waiting for element {Element} on {Url}",
                    waitElements,
                    url);

                return null;
            }
            catch (NoSuchElementException ex)
            {
                _logger.LogWarning(
                    ex,
                    "🔍  Element {Element} not found on {Url}",
                    waitElements,
                    url);

                return null;
            }
            catch (WebDriverException ex)
            {
                _logger.LogError(
                    ex,
                    "🚨  Selenium error while scraping {Url}",
                    url);

                return null;
            }

        }

        public Task<HtmlDocument?> GetHtml(
            string url,
            CancellationToken cancellationToken = default)
        {
            return GetHtml(url, null, cancellationToken);
        }

        private async Task<UndetectedChromeDriver> GetDriver()
        {
            if (_undetectedChromeDriver is not null)
            {
                return _undetectedChromeDriver;
            }

            _undetectedChromeDriver = await CreateDriver();
            return _undetectedChromeDriver;
        }

        public async void Dispose()
        {
            if (_undetectedChromeDriver is not null)
            {
                _undetectedChromeDriver.Quit();
                _undetectedChromeDriver.Dispose();
            }
        }

        private static async Task<UndetectedChromeDriver> CreateDriver()
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