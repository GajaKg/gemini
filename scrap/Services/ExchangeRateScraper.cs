
using gemini.Models;
using gemini.Repositories;
using gemini.Services.CurrencyProviders;
using Microsoft.Extensions.Logging;

namespace gemini.Services
{
    public class ExchangeRateScraper : IExchangeRateScraper
    {
        private readonly ILogger<ExchangeRateScraper> _logger;
        private readonly ICurrencyProvider _currencyProvider;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IExchangeRateRepository _exchangeRateRepository;

        public ExchangeRateScraper(
            ICurrencyProvider currencyProvider,
            ICurrencyRepository currencyRepository,
            IExchangeRateRepository exchangeRateRepository,
            ILogger<ExchangeRateScraper> logger
        )
        {
            _currencyProvider = currencyProvider;
            _currencyRepository = currencyRepository;
            _exchangeRateRepository = exchangeRateRepository;
            _logger = logger;
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
                // Console.WriteLine("❌ There is no currency " + _currencyProvider.CurrencyCode);
                _logger.LogError("❌ There is no currency {CurrencyCode}", _currencyProvider.CurrencyCode);
                return;
            }

            HashSet<DateOnly> existingExchangeDates = (await _exchangeRateRepository.GetAllCurrencyDatesAsync(currency.Id)).ToHashSet();

            _logger.LogInformation("⏳ ⏳ ⏳ {CUrrencyCode} Scraping Started ⏳ ⏳ ⏳", _currencyProvider.CurrencyCode);

            List<ExchangeRate> bulkValues = [];

            for (var date = start; date <= end; date = date.AddDays(1))
            {
                if (existingExchangeDates.Contains(date))
                {
                    _logger.LogWarning("❌ Record already exists!");
                    continue;
                }

                try
                {
                    var exchangeRateRaw = await _currencyProvider.GetExchangeRate(date, cancellationToken);

                    if (exchangeRateRaw is null)
                    {
                        _logger.LogWarning("---- Missing date: {Date:d}", date);
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

                    _logger.LogInformation("🗂  Got currency: {ExchangeRateParsed} for date {Date}, adding for bulk save.", exchangeRateParsed.DetailInfo(), date);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Failed {date}", date);
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

                    // Console.WriteLine($"✅ {bulkValues.Count} Rates are saved in db!!!!!");
                    _logger.LogInformation("✅ {Count} Rates are saved in db!!!!!", bulkValues.Count);
                    bulkValues.Clear();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "❌ Error saving exchange rates!");
                }
                Console.WriteLine("-----------------------------------------------------------");
            }

            if (bulkValues.Count > 0)
            {
                await _exchangeRateRepository.BulkSaveAsync(bulkValues);
            }
            _logger.LogInformation("🟢 {Code} Scraping Completed ⌛️", currency.Code);
        }
    }
}