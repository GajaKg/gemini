using System.Globalization;
using gemini.Interfaces;
using gemini.Models;
using gemini.Repositories;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumUndetectedChromeDriver;

namespace gemini.Services.MAD
{
    public class MADScraperService : IScraperService
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IExchangeRateRepository _exchangeRateRepository;
        private readonly MADParserService _parser;

        private readonly byte bulkSaveNumber = 5; // how many items to save at once
        private readonly string baseUrl = "https://www.bkam.ma/en/Markets/Key-indicators/Foreign-exchange-market/Foreign-exchange-rates/Foreign-banknotes-exchange-rate";

        public MADScraperService(
            ICurrencyRepository currencyRepository,
            IExchangeRateRepository exchangeRateRepository,
            MADParserService parser
        )
        {
            _currencyRepository = currencyRepository;
            _exchangeRateRepository = exchangeRateRepository;
            _parser = parser;
        }

        public async Task RunAsync()
        {

            var currency = await _currencyRepository.GetCurrencyByCode(CurrencyCode.MAD);
            if (currency is null)
            {
                Console.WriteLine("There is no currency " + CurrencyCode.MAD);
                return;
            }

            HashSet<DateOnly> existingExchangeDates = (await _exchangeRateRepository.GetAllCurrencyDatesAsync(currency.Id)).ToHashSet();

            List<ExchangeRate> bulkValues = [];
            byte bulkCounter = 0;

            using var driver = await CreateDriver();

            var start = new DateOnly(2020, 1, 3);
            var end = new DateOnly(2020, 2, 1);
            // var end = new DateOnly(2026, 12, 31);
            for (var date = start; date <= end; date = date.AddDays(1))
            {
                if (existingExchangeDates.Contains(date))
                {
                    Console.WriteLine("Record already exists!");
                    continue;
                }

                var doc = await ScrapeByDate(driver, date);

                if (doc is null) continue;

                var exchangeRates = _parser.Parse(doc, date, currency.Id);

                if (exchangeRates is null)
                {
                    Console.WriteLine("Error parsing data, check for page changes");
                    continue;
                }

                bulkValues.Add(exchangeRates);
                bulkCounter++;

                if (bulkCounter >= bulkSaveNumber)
                {
                    await _exchangeRateRepository.BulkSaveAsync(bulkValues);
                    bulkValues.Clear();
                    bulkCounter = 0;
                    existingExchangeDates.Add(date);
                }
                Console.WriteLine("-----------------------------------------------------------");
            }

            if (bulkValues.Count > 0)
            {
                await _exchangeRateRepository.BulkSaveAsync(bulkValues);
            }

        }

        private async Task<HtmlDocument?> ScrapeByDate(UndetectedChromeDriver driver, DateOnly date)
        {
            // random delay to avoid suspicious behaivour
            int delay = Random.Shared.Next(4000, 10001);
            await Task.Delay(delay);

            string url = $"{baseUrl}?date={date.Day}%2F{date.Month}%2F{date.Year}&block=d1f170603d8b478a6a7b3447ae7f68f3#address-c2e03d492b315ebd7817808fde6acc08-d1f170603d8b478a6a7b3447ae7f68f3";

            try
            {
                await driver.Navigate().GoToUrlAsync(url);
                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(2));
                wait.Until(d => d.FindElements(By.CssSelector(".object_name")).Count > 0);
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine($"No currency data for {date}. Skipping...");
                Console.WriteLine("-----------------------------------------------------------");
                return null;
            }

            if (driver.ExecuteScript("return document.documentElement.outerHTML;") is not string html) return null;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            return doc;
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