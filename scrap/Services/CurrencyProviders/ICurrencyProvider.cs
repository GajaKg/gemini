
using gemini.Interfaces;
using gemini.Models;

namespace gemini.Services.CurrencyProviders
{
    public interface ICurrencyProvider
    {
        CurrencyCode CurrencyCode { get; }
        Task<ExchangeRateRaw?> GetExchangeRate(DateOnly date, CancellationToken cancellationToken = default);
    }
}