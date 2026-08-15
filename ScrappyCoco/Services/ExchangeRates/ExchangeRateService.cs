using System.Net.Http.Json;
using ScrappyCoco.Models;

namespace ScrappyCoco.Services.ExchangeRates;

public class ExchangeRateService : IExchangeRateService
{
    private readonly HttpClient _httpClient;
    private readonly string _url = "/api";

    public ExchangeRateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ExchangeRate>?> GetAllRates()
    {
        return await _httpClient.GetFromJsonAsync<List<ExchangeRate>>(_url + "/currencies");
        // return await _httpClient.GetFromJsonAsync<List<ExchangeRate>>(_url + "/currencies");
    }
}
