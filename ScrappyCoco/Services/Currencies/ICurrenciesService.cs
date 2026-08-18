using ScrappyCoco.Models;

namespace ScrappyCoco.Services.Currencies;

public interface ICurrenciesService
{
    Task<List<Currency>?> GetAllCurrencies();
}