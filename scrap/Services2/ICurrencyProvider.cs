
using gemini.Interfaces;
using gemini.Models;
using HtmlAgilityPack;

namespace gemini.Services2
{
    public interface ICurrencyProvider
    {
        CurrencyCode CurrencyCode { get; }
        // Task<HtmlDocument?> GetHtml(DateOnly date, CancellationToken cancellationToken = default);
        // ExchangeRate? Parse(HtmlDocument doc, DateOnly date, int currencyId);
        Task<ExchangeRateRaw?> GetExchangeRate(DateOnly date, CancellationToken cancellationToken = default);
    }
}