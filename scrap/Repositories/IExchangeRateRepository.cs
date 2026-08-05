using gemini.Dtos;
using gemini.Models;

namespace gemini.Repositories
{
    public interface IExchangeRateRepository
    {
        Task BulkSaveAsync(List<ExchangeRate> items);
        Task<IEnumerable<ExchangeRateLookup>> GetAllCurrencyDatesAsync(int currencyId);
    }
}