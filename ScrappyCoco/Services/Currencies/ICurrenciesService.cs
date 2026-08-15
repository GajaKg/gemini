using ScrappyCoco.Models;

namespace ScrappyCoco.Services.Currencies;

public interface ICurrenciesService
{
    Task<Currency?> GetCurrencyAndRatesByTargetId(int id, int targetCurrencyId);
    Task<List<Currency>?> GetAllCurrencies();
}