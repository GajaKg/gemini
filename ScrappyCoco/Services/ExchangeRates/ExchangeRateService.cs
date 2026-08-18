using System.Net.Http.Json;
using ScrappyCoco.Models;

namespace ScrappyCoco.Services.ExchangeRates;

public class ExchangeRateService : IExchangeRateService
{
    private readonly HttpClient _httpClient;
    private readonly string _url = "/api/exchangeRates";

    public ExchangeRateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResponse<ExchangeRate>?> GetAllRates(int currencyForId, int currencyTargetId, PaginationParams paginationParams, DateTime? searchDate)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["Id"] = currencyForId.ToString(),
            ["TargetCurrencyId"] = currencyTargetId.ToString(),
            ["CurrentPage"] = paginationParams.CurrentPage.ToString(),
            ["PageSize"] = paginationParams.PageSize.ToString(),
        };

        if (searchDate.HasValue)
        {
            queryParams["Date"] = DateOnly
                                    .FromDateTime(searchDate.Value)
                                    .ToString("yyyy-MM-dd");
        }

        var queryString = await new FormUrlEncodedContent(queryParams).ReadAsStringAsync();
        string fullUrl = $"{_url}?{queryString}";

        return await _httpClient.GetFromJsonAsync<PagedResponse<ExchangeRate>>(fullUrl);
    }
}
