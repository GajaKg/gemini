using ScrappyCoco.Models;

namespace ScrappyCoco.Services.ExchangeRates;

public interface IExchangeRateService
{
    public Task<PagedResponse<ExchangeRate>?> GetAllRates(int currencyForId, int currencyTargetId);
}