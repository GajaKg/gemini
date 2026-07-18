using System.Globalization;
using gemini.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumUndetectedChromeDriver;

namespace gemini.Services
{
    public class MADScraperService : IScraperService
    {
        private readonly HttpClient _httpClient;
        private readonly IParserService _parser;


        public MADScraperService(
            HttpClient httpClient,
            IParserService parser)
        {
            _httpClient = httpClient;
            _parser = parser;
        }

        public async Task RunAsync()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--headless=new");

            var driver = UndetectedChromeDriver.Create(
                options: chromeOptions,
                driverExecutablePath: await new ChromeDriverInstaller().Auto()
            );


            string baseUrl = "https://www.bkam.ma/en/Markets/Key-indicators/Foreign-exchange-market/Foreign-exchange-rates/Foreign-banknotes-exchange-rate";
            var start = new DateOnly(2020, 1, 3);
            var end = new DateOnly(2020, 2, 1);
            // var end = new DateOnly(2026, 12, 31);

            for (var date = start; date <= end; date = date.AddDays(1))
            {
                string url = $"{baseUrl}?date={date.Day}%2F{date.Month}%2F{date.Year}&block=d1f170603d8b478a6a7b3447ae7f68f3#address-c2e03d492b315ebd7817808fde6acc08-d1f170603d8b478a6a7b3447ae7f68f3";
                await Scrap(driver, url, date);
            }

        }
        public async Task Scrap(ChromeDriver driver, string url, DateOnly date)
        {
            try
            {
                // Navigate to your protected currency rate website
                await driver.Navigate().GoToUrlAsync(url);
                var htmlContent = driver.ExecuteScript("return document.documentElement.outerHTML;") as string;
                // Console.WriteLine(htmlContent);
                File.WriteAllText($"debug-{date}.html", htmlContent);
                Console.WriteLine("Title: " + driver.Title);
                Console.WriteLine("URL: " + driver.Url);
                // await Task.Delay(10000);
                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(2));

                try
                {
                    wait.Until(d => d.FindElements(By.CssSelector(".object_name")).Count > 0);
                }
                catch (WebDriverTimeoutException)
                {
                    Console.WriteLine($"No currency data for {date}. Skipping...");
                    Console.WriteLine("-----------------------------------------------------------");
                    return; // continue next loop
                }

                IWebElement row = driver.FindElement(By.CssSelector("tbody > tr:first-child"));

                if (row is null)
                {
                    Console.WriteLine($"No currency data for {date}. row Skipping...");
                    return;
                }

                string? currency = row.FindElement(By.CssSelector(".object_name"))?.Text;
                string? purchaseValue = row.FindElement(By.CssSelector("td:nth-child(2) .number"))?.Text.Trim();
                string? sellValue = row.FindElement(By.CssSelector("td:last-child .number"))?.Text.Trim();
                Console.WriteLine(currency);
                Console.WriteLine(purchaseValue);
                Console.WriteLine(sellValue);


                if (
                    currency == CurrencyNames.Euro &&
                    purchaseValue != null &&
                    sellValue != null
                )
                {
                    decimal purchase = decimal.Parse(purchaseValue, CultureInfo.InvariantCulture);
                    decimal sell = decimal.Parse(sellValue, CultureInfo.InvariantCulture);
                    decimal middleCourse = (purchase + sell) / 2;
                    Console.WriteLine("Srednji kurs na dan " + date.ToString() + ": " + middleCourse);
                }

                // foreach (var t in divvs)
                // {
                //     Console.WriteLine(t.Text);
                // }

            }
            catch (System.Exception)
            {
                Console.WriteLine("AISDOAISYTOAISUYTIUASYT");
                throw;
            }
            Console.WriteLine("-----------------------------------------------------------");
            // var chromeOptions = new ChromeOptions();
            // chromeOptions.AddArguments("headless");

            // var driver = new ChromeDriver(chromeOptions);

            // // await driver.Navigate().GoToUrlAsync(_url);
            // await driver.Navigate().GoToUrlAsync("https://formaideale.rs/novo");

            // // var productHTMLElements = driver.FindElement(By.CssSelector("li.bullet2"));
            // var productHTMLElements = driver.FindElements(By.CssSelector("article"));
            // Console.WriteLine(productHTMLElements);
            // // wait.Until(d => d.FindElements(By.CssSelector("table")).Count > 0);
            // // IReadOnlyCollection<IWebElement> rows = driver.FindElements(By.CssSelector("tr"));
        }


    }
}