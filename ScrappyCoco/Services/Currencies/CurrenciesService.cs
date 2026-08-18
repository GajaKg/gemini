using System.Net.Http.Json;
using ScrappyCoco.Models;

namespace ScrappyCoco.Services.Currencies;

public class CurrenciesService : ICurrenciesService
{
    private readonly HttpClient _httpClient;
    private readonly string _url = "/api/currencies";

    public CurrenciesService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Currency>?> GetAllCurrencies()
    {
        return await _httpClient.GetFromJsonAsync<List<Currency>>(_url);
    }
}