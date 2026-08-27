using gemini.Dtos;
using Scrap.Domain.Entities;

namespace gemini.Repositories
{
    public interface IExchangeRateRepository
    {
        Task BulkSaveAsync(List<ExchangeRate> items);
        Task<IEnumerable<ExchangeRateLookup>> GetAllRatesDatesAsync(int currencyId);
    }
}