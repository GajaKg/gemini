namespace gemini.Services
{
    public interface IExchangeRateScraper
    {
        Task ScrapeDateRange(DateOnly start, DateOnly end, int? bulkSaveNumber = null, CancellationToken cancellationToken = default);
        Task ScrapeLastDays(int lastDays, int? bulkSaveNumber = null, CancellationToken cancellationToken = default);
    }
}