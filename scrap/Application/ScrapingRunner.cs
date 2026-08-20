using gemini.Services;
using gemini.Services.CurrencyProviders;
using Microsoft.Extensions.Logging;

namespace gemini.Application;

public class ScrapingRunner
{
    private readonly IEnumerable<IExchangeRateScraper<ICurrencyProvider>> _scrapers;
    private readonly ILogger<ScrapingRunner> _logger;

    public ScrapingRunner(
        IEnumerable<IExchangeRateScraper<ICurrencyProvider>> scrapers,
        ILogger<ScrapingRunner> logger)
    {
        _scrapers = scrapers;
        _logger = logger;
    }

    public async Task RunScrapeLastDaysAsync(int lastDaysNumber, CancellationToken cancellationToken = default)
    {
        foreach (var scraper in _scrapers)
        {
            try
            {
                await scraper.ScrapeLastDays(lastDaysNumber, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scraper failed");
            }
        }
    }
    public async Task RunScrapeDateRangeAsync(DateOnly from, DateOnly to, int bulkSaveNumber = 10, CancellationToken cancellationToken = default)
    {
        foreach (var scraper in _scrapers)
        {
            try
            {
                await scraper.ScrapeDateRange(from, to, bulkSaveNumber, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scraper failed");
            }
        }
    }
}