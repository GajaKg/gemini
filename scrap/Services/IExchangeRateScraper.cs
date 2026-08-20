using gemini.Services.CurrencyProviders;

namespace gemini.Services;

public interface IExchangeRateScraper<TProvider>
    where TProvider : ICurrencyProvider
{
    Task ScrapeDateRange(DateOnly start, DateOnly end, int bulkSaveNumber, CancellationToken cancellationToken = default);
    Task ScrapeDateRange(DateOnly start, DateOnly end, CancellationToken cancellationToken = default);
    Task ScrapeLastDays(int lastDays, int bulkSaveNumber, CancellationToken cancellationToken = default);
    Task ScrapeLastDays(int lastDays, CancellationToken cancellationToken = default);
}