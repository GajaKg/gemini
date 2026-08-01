
using gemini.Models;
using gemini.Repositories;
using gemini.Services.CurrencyProviders;

namespace gemini.Services
{
    public class ExchangeRateScraper : IExchangeRateScraper
    {
        private readonly ICurrencyProvider _currencyProvider;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IExchangeRateRepository _exchangeRateRepository;

        public ExchangeRateScraper(
            ICurrencyProvider currencyProvider,
            ICurrencyRepository currencyRepository,
            IExchangeRateRepository exchangeRateRepository
        )
        {
            _currencyProvider = currencyProvider;
            _currencyRepository = currencyRepository;
            _exchangeRateRepository = exchangeRateRepository;
        }

        public async Task ScrapeDateRange(DateOnly start, DateOnly end, int? bulkSaveNumber = 10, CancellationToken cancellationToken = default)
        {
            int bulkSize = bulkSaveNumber ?? 10;
            await Scrape(start, end, bulkSize, cancellationToken);

        }

        public async Task ScrapeLastDays(int lastDays, int? bulkSaveNumber = 5, CancellationToken cancellationToken = default)
        {
            int bulkSize = bulkSaveNumber ?? 5;
            DateOnly end = DateOnly.FromDateTime(DateTime.Today);
            DateOnly start = end.AddDays(-(lastDays - 1));

            await Scrape(start, end, bulkSize, cancellationToken);
        }

        public async Task Scrape(
            DateOnly start,
            DateOnly end,
            int? bulkSaveNumber = 10,
            CancellationToken cancellationToken = default
        )
        {
            var currency = await _currencyRepository.GetCurrencyByCode(_currencyProvider.CurrencyCode);
            if (currency is null)
            {
                Console.WriteLine("❌ There is no currency " + _currencyProvider.CurrencyCode);
                return;
            }

            HashSet<DateOnly> existingExchangeDates = (await _exchangeRateRepository.GetAllCurrencyDatesAsync(currency.Id)).ToHashSet();

            Console.WriteLine($"⏳ ⏳ ⏳ {_currencyProvider.CurrencyCode} Scraping Started ⏳ ⏳ ⏳");

            List<ExchangeRate> bulkValues = [];

            for (var date = start; date <= end; date = date.AddDays(1))
            {
                if (existingExchangeDates.Contains(date))
                {
                    Console.WriteLine("❌ Record already exists!");
                    continue;
                }

                try
                {
                    var exchangeRateRaw = await _currencyProvider.GetExchangeRate(date, cancellationToken);

                    if (exchangeRateRaw is null)
                    {
                        Console.WriteLine($"---- Missing date {date:d}");
                        continue;
                    }

                    var exchangeRateParsed = new ExchangeRate
                    {
                        CurrencyId = currency.Id,
                        Sell = exchangeRateRaw.Sell,
                        Buy = exchangeRateRaw.Buy,
                        Middle = exchangeRateRaw.Middle,
                        Date = date
                    };

                    bulkValues.Add(exchangeRateParsed);

                    Console.WriteLine($"🗂  Got currency: {exchangeRateParsed.DetailInfo()}, adding for bulk save.");
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"❌ Failed {date}: {ex.Message}");
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

                    Console.WriteLine($"✅ {bulkValues.Count} Rates are saved in db!!!!!");
                    bulkValues.Clear();
                }
                catch (System.Exception e)
                {
                    Console.WriteLine($"❌ Error saving exchange rates! {e}");
                    throw;
                }
                Console.WriteLine("-----------------------------------------------------------");
            }

            if (bulkValues.Count > 0)
            {
                await _exchangeRateRepository.BulkSaveAsync(bulkValues);
            }
            Console.WriteLine($"🟢 {currency.Code} Scraping Completed ⌛️");
        }
    }
}