
using gemini.Interfaces;
using gemini.Models;
using gemini.Repositories;
using HtmlAgilityPack;

namespace gemini.Services.XOF
{
    public class XOFScraperService : IScraperService
    {
        // https://www.bceao.int/en/cours/cours-des-devises-contre-Franc-CFA-appliquer-aux-transferts";

        private readonly string baseUrl = "https://www.bceao.int/en/cours/get_all_devise_by_date";
        private readonly byte bulkSaveNumber = 5; // how many items to save at once
        private readonly HttpClient _httpClient;
        private readonly XOFParserService _parser;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IExchangeRateRepository _exchangeRateRepository;

        public XOFScraperService(
            HttpClient httpClient,
            XOFParserService parser,
            ICurrencyRepository currencyRepository,
            IExchangeRateRepository exchangeRateRepository
        )
        {
            _currencyRepository = currencyRepository;
            _exchangeRateRepository = exchangeRateRepository;
            _parser = parser;
            _httpClient = httpClient;
        }

        public async Task RunAsync()
        {
            Console.WriteLine("XOF Scraping started....");

            Currency? currency = await _currencyRepository.GetCurrencyByCode(CurrencyCode.XOF);
            if (currency == null)
            {
                Console.WriteLine("There is no wanted currency");
                return;
            }

            HashSet<DateOnly> existingExchangeDates = (await _exchangeRateRepository.GetAllCurrencyDatesAsync(currency.Id)).ToHashSet(); ;

            List<ExchangeRate> bulkValues = [];
            byte bulkCounter = 0;

            DateOnly start = new(2020, 1, 1);
            DateOnly end = DateOnly.FromDateTime(DateTime.Today);

            for (var date = start; date <= end; date = date.AddDays(1))
            {
                if (existingExchangeDates.Contains(date))
                {
                    Console.WriteLine("Record already exists!");
                    continue;
                }

                HtmlDocument doc = await ScrapeByDate(date);

                // extracting data from html
                ExchangeRate? exchangeRateParsed = _parser.Parse(doc, date, currency.Id);

                if (exchangeRateParsed != null)
                {
                    bulkValues.Add(exchangeRateParsed);
                    bulkCounter++;

                    Console.WriteLine(date.ToShortDateString());
                    Console.WriteLine(exchangeRateParsed?.ToString());
                    // save after collecting {bulkSaveNumber} items
                    if (bulkCounter >= bulkSaveNumber)
                    {
                        try
                        {
                            await _exchangeRateRepository.BulkSaveAsync(bulkValues);
                            bulkCounter = 0;
                            bulkValues.Clear();
                            existingExchangeDates.Add(date);
                            Console.WriteLine($"{bulkCounter} Rates are saved in db!!!!!");
                        }
                        catch (System.Exception)
                        {
                            Console.WriteLine("Error saving exchange rates!");
                            throw;
                        }
                    }
                }
            }

            // save rest values
            if (bulkValues.Count > 0)
            {
                await _exchangeRateRepository.BulkSaveAsync(bulkValues);
            }
            Console.WriteLine("-----------------------------------------------------------");
        }

        private async Task<HtmlDocument> ScrapeByDate(DateOnly date)
        {
            // https://www.bceao.int/en/cours/get_all_devise_by_date?dateJour=2020-05-07";
            // url returns html table with rates on requested day
            string urlByDay = $"{baseUrl}?dateJour={date.Year}-{date.Month}-{date.Day}";

            // random delay to avoid suspicious behaivour
            int delay = Random.Shared.Next(4000, 10001);
            await Task.Delay(delay);

            // geting page html
            string html = await _httpClient.GetStringAsync(urlByDay);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            return doc;
        }
    }
}