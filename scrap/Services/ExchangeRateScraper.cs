
using gemini.Interfaces;
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
            var euro = await _currencyRepository.GetCurrencyByCode(CurrencyCode.EUR);
            var usd = await _currencyRepository.GetCurrencyByCode(CurrencyCode.USD);

            var targetCurrencies = new Dictionary<CurrencyCode, Currency>();
            if (euro is not null) targetCurrencies.Add(CurrencyNames.EUR, euro);
            if (usd is not null) targetCurrencies.Add(CurrencyNames.USD, usd);

            var currency = await _currencyRepository.GetCurrencyByCode(_currencyProvider.CurrencyCode);
            if (currency is null)
            {
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
                    var exchangeRatesRaw = await _currencyProvider.GetExchangeRate(date, cancellationToken);

                    if (exchangeRatesRaw is null || exchangeRatesRaw.Count < 1)
                    {
                        _logger.LogWarning("---- Missing date: {Date:d}", date);
                        continue;
                    }

                    foreach (var rawCurrency in exchangeRatesRaw)
                    {
                        if (!targetCurrencies.TryGetValue(
                                rawCurrency.TargetCurrency,
                                out var targetCurrency))
                        {
                            _logger.LogWarning(
                                "⚠️  Currency {Currency} is not configured",
                                rawCurrency.TargetCurrency
                            );

                            continue;
                        }

                        var exchangeRateParsed = new ExchangeRate
                        {
                            TargetCurrencyId = targetCurrency.Id,
                            CurrencyId = currency.Id,
                            Sell = rawCurrency.Sell,
                            Buy = rawCurrency.Buy,
                            Middle = rawCurrency.Middle,
                            Date = date
                        };

                        bulkValues.Add(exchangeRateParsed);

                        _logger.LogInformation("🗂  Got rates({TargetCurrency} to {Currency}): {ExchangeRateParsed} on date {Date}, adding for bulk save.",
                            targetCurrency.Name,
                            currency.Code,
                            exchangeRateParsed.DetailInfo(),
                            date
                        );
                    }

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