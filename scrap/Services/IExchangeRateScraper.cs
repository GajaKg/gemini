namespace gemini.Services;

public interface IExchangeRateScraper
{
    Task ScrapeDateRange(DateOnly start, DateOnly end, int bulkSaveNumber, CancellationToken cancellationToken = default);
    Task ScrapeDateRange(DateOnly start, DateOnly end, CancellationToken cancellationToken = default);
    Task ScrapeLastDays(int lastDays, int bulkSaveNumber, CancellationToken cancellationToken = default);
    Task ScrapeLastDays(int lastDays, CancellationToken cancellationToken = default);
}