using gemini.Dtos;
using gemini.Models;

namespace gemini.Repositories
{
    public interface IExchangeRateRepository
    {
        Task BulkSaveAsync(List<ExchangeRate> items);
        Task<IEnumerable<DateOnly>> GetAllCurrencyDatesAsync(int currencyId);
    }
}