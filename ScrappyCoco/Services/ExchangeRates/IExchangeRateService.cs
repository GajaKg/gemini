using ScrappyCoco.Models;

namespace ScrappyCoco.Services.ExchangeRates;

public interface IExchangeRateService
{
    public Task<List<ExchangeRate>?> GetAllRates();
}