using ScrappyCoco.Models;

namespace ScrappyCoco.Services.ExchangeRates;

public interface IExchangeRateService
{
    public Task<PagedResponse<ExchangeRate>?> GetAllRates(ExchangeRateParams exchangeRateParams, CancellationToken cancellationToken);
}