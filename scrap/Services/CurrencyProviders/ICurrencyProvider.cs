using Scrap.Domain.Interfaces;
using Scrap.Domain.Models;

namespace gemini.Services.CurrencyProviders;

public interface ICurrencyProvider
{
    CurrencyCode CurrencyCode { get; }
    Task<List<ExchangeRateRaw>?> GetExchangeRate(DateOnly date, CancellationToken cancellationToken = default);
}

public interface IXofCurrencyProvider : ICurrencyProvider;

public interface IMadCurrencyProvider : ICurrencyProvider;