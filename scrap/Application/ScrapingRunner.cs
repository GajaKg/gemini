using gemini.Services;
using Microsoft.Extensions.Logging;

namespace gemini.Application;

public class ScrapingRunner
{
    private readonly IEnumerable<IExchangeRateScraper> _scrapers;
    private readonly ILogger<ScrapingRunner> _logger;

    public ScrapingRunner(
        IEnumerable<IExchangeRateScraper> scrapers,
        ILogger<ScrapingRunner> logger)
    {
        _scrapers = scrapers;
        _logger = logger;
    }

    public async Task RunScrapeLastDaysAsync(CancellationToken cancellationToken = default)
    {
        foreach (var scraper in _scrapers)
        {
            try
            {
                await scraper.ScrapeLastDays(10, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scraper failed");
            }
        }
    }
    public async Task RunScrapeDateRangeAsync(CancellationToken cancellationToken = default)
    {
        foreach (var scraper in _scrapers)
        {
            try
            {
                await scraper.ScrapeDateRange(new DateOnly(2020, 1, 1), DateOnly.FromDateTime(DateTime.Today), 10, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scraper failed");
            }
        }
    }
}