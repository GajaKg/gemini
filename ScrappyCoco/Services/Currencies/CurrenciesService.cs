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

    public async Task<Currency?> GetCurrencyAndRatesByTargetId(int id, int targetCurrencyId)
    {
        // 1. Define your query parameters
        var queryParams = new Dictionary<string, string?>
        {
            ["Id"] = id.ToString(),
            ["TargetCyrrencyId"] = targetCurrencyId.ToString(),
        };
        // Convert dictionary to search?search=laptop&page=2
        var queryString = await new FormUrlEncodedContent(queryParams).ReadAsStringAsync();
        string fullUrl = $"{_url}/list?{queryString}";

        return await _httpClient.GetFromJsonAsync<Currency>(fullUrl);
    }

    public async Task<List<Currency>?> GetAllCurrencies()
    {
        return await _httpClient.GetFromJsonAsync<List<Currency>>(_url);
    }
}