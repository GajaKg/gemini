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

    public async Task<PagedResponse<ExchangeRate>?> GetAllRates(ExchangeRateParams exchangeRateParams, CancellationToken cancellationToken)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["Id"] = exchangeRateParams.CurrencyForId.ToString(),
            ["TargetCurrencyId"] = exchangeRateParams.CurrencyTargetId.ToString(),
            ["CurrentPage"] = exchangeRateParams.CurrentPage.ToString(),
            ["PageSize"] = exchangeRateParams.PageSize.ToString(),
        };

        if (exchangeRateParams.SearchDate.HasValue)
        {
            queryParams["Date"] = DateOnly
                                    .FromDateTime(exchangeRateParams.SearchDate.Value)
                                    .ToString("yyyy-MM-dd");
        }

        var queryString = await new FormUrlEncodedContent(queryParams).ReadAsStringAsync(cancellationToken);
        string fullUrl = $"{_url}?{queryString}";

        try
        {
            return await _httpClient.GetFromJsonAsync<PagedResponse<ExchangeRate>>(
                fullUrl,
                cancellationToken);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
