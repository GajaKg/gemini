using System.Net;
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
            var response = await _httpClient.GetAsync(
               fullUrl,
               cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content
                    .ReadFromJsonAsync<PagedResponse<ExchangeRate>>(
                        cancellationToken);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var problem = await response.Content
                    .ReadFromJsonAsync<ValidationProblemResponse>(
                        cancellationToken);

                throw new ApiValidationException(
                    (int)response.StatusCode,
                    problem?.Errors ?? [],
                    problem?.Title);
            }

            response.EnsureSuccessStatusCode();

            return null;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
    }
}
