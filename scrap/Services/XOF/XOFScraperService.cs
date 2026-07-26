
using gemini.Interfaces;
using gemini.Models;
using gemini.Repositories;
using HtmlAgilityPack;

namespace gemini.Services.XOF
{
    public class XOFScraperService : IScraperService
    {
        // https://www.bceao.int/en/cours/cours-des-devises-contre-Franc-CFA-appliquer-aux-transferts";

        private const string baseUrl = "https://www.bceao.int/en/cours/get_all_devise_by_date";
        private readonly HttpClient _httpClient;
        private readonly IParserService _parser;
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

        public async Task ScrapeDateRange(DateOnly start, DateOnly end, int? bulkSaveNumber = 10, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("XOF Scraping started....");

            int bulkSize = bulkSaveNumber ?? 10;
            await Scrape(start, end, bulkSize, cancellationToken);

            Console.WriteLine("-----------------------------------------------------------");
        }

        public async Task ScrapeLastDays(int lastDays, int? bulkSaveNumber = 5, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("XOF Scraping started....");

            int bulkSize = bulkSaveNumber ?? 5;
            DateOnly end = DateOnly.FromDateTime(DateTime.Today);
            DateOnly start = end.AddDays(-(lastDays - 1));

            await Scrape(start, end, bulkSize, cancellationToken);

            Console.WriteLine("-----------------------------------------------------------");
        }

        private async Task Scrape(DateOnly start, DateOnly end, int? bulkSaveNumber = 10, CancellationToken cancellationToken = default)
        {
            // check if currency exists
            Currency? currency = await _currencyRepository.GetCurrencyByCode(CurrencyCode.XOF);
            if (currency == null)
            {
                Console.WriteLine("There is no wanted currency");
                return;
            }

            // getting all saved exchange rate dates
            // to prevent duplicate save error
            HashSet<DateOnly> existingExchangeDates = (
                await _exchangeRateRepository.GetAllCurrencyDatesAsync(currency.Id)
            ).ToHashSet();

            List<ExchangeRate> bulkValues = [];

            for (var date = start; date <= end; date = date.AddDays(1))
            {
                Console.WriteLine($"Checking exchange rates for a day: {date}");
                // check if rate exists for a given date, is so skip to next day
                if (existingExchangeDates.Contains(date))
                {
                    Console.WriteLine("Record already exists!");
                    continue;
                }

                try
                {
                    // get html
                    HtmlDocument? doc = await GetHtmlByDate(date, cancellationToken);
                    if (doc is null) continue;

                    // extracting data from html
                    ExchangeRate? exchangeRateParsed = _parser.Parse(doc, date, currency.Id);
                    if (exchangeRateParsed is null) continue;

                    bulkValues.Add(exchangeRateParsed);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"Failed {date}: {ex.Message}");
                }

                // if criteria is not met then dont save exchange rates
                if (bulkValues.Count < bulkSaveNumber) continue;

                try
                {
                    // save after collecting {bulkSaveNumber} items
                    await _exchangeRateRepository.BulkSaveAsync(bulkValues);

                    // update list of existing exchange rates
                    foreach (var item in bulkValues)
                    {
                        existingExchangeDates.Add(item.Date);
                    }

                    Console.WriteLine($"{bulkValues.Count} Rates are saved in db!!!!!");
                    bulkValues.Clear();
                }
                catch (System.Exception e)
                {
                    Console.WriteLine($"Error saving exchange rates! {e}");
                    throw;
                }
            }

            // save remaining values
            if (bulkValues.Count > 0)
            {
                try
                {
                    await _exchangeRateRepository.BulkSaveAsync(bulkValues);
                    Console.WriteLine($"{bulkValues.Count} Rates are saved in db!!!!!");
                }
                catch (System.Exception e)
                {
                    Console.WriteLine($"Error saving exchange rates! {e}");
                }
            }
        }

        private async Task<HtmlDocument?> GetHtmlByDate(DateOnly date, CancellationToken cancellationToken = default)
        {
            // url returns html table with rates on requested day
            // https://www.bceao.int/en/cours/get_all_devise_by_date?dateJour=2020-05-07";
            string urlByDay = $"{baseUrl}?dateJour={date.Year}-{date.Month}-{date.Day}";

            // random delay to avoid suspicious behaivour
            int delay = Random.Shared.Next(4000, 10001);
            await Task.Delay(delay, cancellationToken);

            // geting page html
            try
            {
                string html = await _httpClient.GetStringAsync(urlByDay, cancellationToken);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                return doc;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Failed to download {date:yyyy-MM-dd}: {ex.Message}");
                return null;
            }
        }

    }
}