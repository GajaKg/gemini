
using gemini.Dtos;
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

        private const int DefaultBulkSaveNumber = 10;

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

        /// <summary>
        /// Scrape from date to date
        /// </summary>
        /// <param name="bulkSaveNumber">Save rates in database after {bulkSaveNumber} rates.</param>
        public async Task ScrapeDateRange(DateOnly start, DateOnly end, int bulkSaveNumber, CancellationToken cancellationToken = default)
        {
            await Scrape(start, end, bulkSaveNumber, cancellationToken);
        }
        public async Task ScrapeDateRange(DateOnly start, DateOnly end, CancellationToken cancellationToken = default)
        {
            await Scrape(start, end, DefaultBulkSaveNumber, cancellationToken);
        }

        /// <summary>
        /// Scrape from date to date
        /// </summary>
        /// <param name="lastDays">Scrape eg last 7 days rates from today.</param>
        /// <param name="bulkSaveNumber">Save rates in database after {bulkSaveNumber} rates.</param>
        public async Task ScrapeLastDays(int lastDays, int bulkSaveNumber, CancellationToken cancellationToken = default)
        {
            DateOnly end = DateOnly.FromDateTime(DateTime.Today);
            DateOnly start = end.AddDays(-(lastDays - 1));

            await Scrape(start, end, bulkSaveNumber, cancellationToken);
        }

        public Task ScrapeLastDays(
            int lastDays,
            CancellationToken cancellationToken = default)
        {
            return ScrapeLastDays(lastDays, DefaultBulkSaveNumber, cancellationToken);
        }

        /// <summary>
        /// main scraper
        /// </summary>
        public async Task Scrape(
            DateOnly start,
            DateOnly end,
            int bulkSaveNumber = 10,
            CancellationToken cancellationToken = default
        )
        {
            var context = await InitializeScrapeContext(cancellationToken);
            if (context is null) return;

            // currency (XOF, MAD...)
            // targetCurrencies (USD, EUR...)
            // 
            var (currency, targetCurrencies, existingExchanges) = context.Value;

            _logger.LogInformation("⏳ ⏳ ⏳ {CUrrencyCode} Scraping Started ⏳ ⏳ ⏳", _currencyProvider.CurrencyCode);

            List<ExchangeRate> bulkValues = [];

            for (var date = start; date <= end; date = date.AddDays(1))
            {
                try
                {
                    var exchangeRatesRaw = await _currencyProvider.GetExchangeRate(date, cancellationToken);

                    if (exchangeRatesRaw is null || exchangeRatesRaw.Count < 1)
                    {
                        _logger.LogWarning("No Currency {Currency}", currency.Code);
                        _logger.LogWarning("---- Missing date: {Date:d}", date);
                        continue;
                    }

                    ProccessCurrencies(
                        exchangeRatesRaw,
                        targetCurrencies,
                        existingExchanges,
                        currency,
                        bulkValues,
                        date
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Failed {date}", date);
                }

                // if criteria is not met then dont save rates
                if (bulkValues.Count < bulkSaveNumber) continue;

                await SaveBulkValuesAsync(bulkValues, existingExchanges);

                Console.WriteLine("-----------------------------------------------------------");
            }

            await SaveBulkValuesAsync(bulkValues, existingExchanges);

            _logger.LogInformation("🟢 {Code} Scraping Completed ⌛️", currency.Code);
        }

        /// <summary>
        /// Get all currencies and saved exchange rates
        /// </summary>
        private async Task<(
            Currency SourceCurrency,
            Dictionary<CurrencyCode, Currency> TargetCurrencies,
            HashSet<ExchangeRateLookup> ExistingRates
        )?> InitializeScrapeContext(CancellationToken cancellationToken)
        {
            var currencies = await _currencyRepository.GetAllAsync(cancellationToken);

            // Check if MAD and XOF exists in db
            var currency = currencies.SingleOrDefault(c => c.Code == _currencyProvider.CurrencyCode);
            if (currency is null)
            {
                _logger.LogError("❌ There is no currency {CurrencyCode}", _currencyProvider.CurrencyCode);
                return null;
            }

            var targetCurrencies = new Dictionary<CurrencyCode, Currency>();

            var euro = currencies.SingleOrDefault(c => c.Code == CurrencyCode.EUR);
            var usd = currencies.SingleOrDefault(c => c.Code == CurrencyCode.USD);
            if (euro is not null) targetCurrencies.Add(CurrencyNames.EUR, euro);
            if (usd is not null) targetCurrencies.Add(CurrencyNames.USD, usd);

            // get all rates for given currency (MAD, XOF) 
            // to avoid duplicate entries
            HashSet<ExchangeRateLookup> existingExchanges = (
                await _exchangeRateRepository.GetAllRatesDatesAsync(currency.Id)
            ).ToHashSet();

            return (
                currency,
                targetCurrencies,
                existingExchanges
            );

        }

        /// <summary>
        /// Proccess list of rates received from currencyProvider
        /// </summary>
        private void ProccessCurrencies(
            List<ExchangeRateRaw> exchangeRatesRaw,
            Dictionary<CurrencyCode, Currency> targetCurrencies,
            HashSet<ExchangeRateLookup> existingExchanges,
            Currency currency,
            List<ExchangeRate> bulkValues,
            DateOnly date)
        {
            foreach (var rawCurrency in exchangeRatesRaw)
            {
                // check for EUR and USD
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

                // create object for existing rate lookup
                // check for date and targetCurrency.id (USD, EUR... id)
                var newRate = new ExchangeRateLookup
                {
                    Date = date,
                    TargetCurrencyId = targetCurrency.Id
                };

                if (existingExchanges.Contains(newRate))
                {
                    _logger.LogWarning(
                        "❌ Exchange rate ({From} to {To}) for a date {Date} already exists!",
                        targetCurrency.Name,
                        currency.Code,
                        date
                    );
                    continue;
                }

                // create rate model
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

                _logger.LogInformation("✅🗂  Got rates({TargetCurrency} to {Currency}): {ExchangeRateParsed} on date {Date}, adding for bulk save.",
                    targetCurrency.Code,
                    currency.Code,
                    exchangeRateParsed.DetailInfo(),
                    date
                );
            }
        }

        private async Task SaveBulkValuesAsync(
            List<ExchangeRate> bulkValues,
            HashSet<ExchangeRateLookup> existingExchanges
        )
        {
            if (bulkValues.Count < 1) return;

            try
            {
                // save after collecting {bulkSaveNumber} items
                await _exchangeRateRepository.BulkSaveAsync(bulkValues);

                // update list of existing exchange rates
                foreach (var item in bulkValues)
                {
                    existingExchanges.Add(
                        new ExchangeRateLookup
                        {
                            Date = item.Date,
                            TargetCurrencyId = item.TargetCurrencyId
                        }
                    );
                }

                _logger.LogInformation("✅ {Count} rates are saved in db!!!!!", bulkValues.Count);
                bulkValues.Clear();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "❌ Error saving exchange rates!");
            }
        }

    }
}