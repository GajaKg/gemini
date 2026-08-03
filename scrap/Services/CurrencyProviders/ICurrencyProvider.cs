
using gemini.Interfaces;
using gemini.Models;

namespace gemini.Services.CurrencyProviders
{
    public interface ICurrencyProvider
    {
        CurrencyCode CurrencyCode { get; }
        Task<List<ExchangeRateRaw>?> GetExchangeRate(DateOnly date, CancellationToken cancellationToken = default);
    }
}