using gemini.Services.CurrencyParser;
using gemini.Services.HtmlProviders;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Scrap.Domain.Enums;
using Scrap.Domain.Models;

namespace gemini.Services.CurrencyProviders
{

    public class XofProvider : IXofCurrencyProvider
    {
        public CurrencyCode CurrencyCode => CurrencyCode.XOF;

        private const string baseUrl = "https://www.bceao.int/en/cours/get_all_devise_by_date";

        private readonly IHttpClientProvider _driverProvider;
        private readonly IXofParserService _parserService;

        public XofProvider(IHttpClientProvider driverProvider, IXofParserService parserService, ILogger<XofProvider> logger)
        {
            _driverProvider = driverProvider;
            _parserService = parserService;
        }

        /// <summary>
        /// Get exchange rates
        /// </summary>
        /// <param name="date">Used in url to get rates at date</param>
        public async Task<List<ExchangeRateRaw>?> GetExchangeRate(DateOnly date, CancellationToken cancellationToken = default)
        {
            // Creates random delay to avoid scraping detection
            int delay = Random.Shared.Next(4000, 10001);
            await Task.Delay(delay, cancellationToken);

            string urlByDay = $"{baseUrl}?dateJour={date.Year}-{date.Month}-{date.Day}";

            // get html
            HtmlDocument? doc = await _driverProvider.GetHtml(urlByDay, cancellationToken);
            if (doc is null) return null;

            // extracting data from html
            return _parserService.Parse(doc);
        }
    }
}