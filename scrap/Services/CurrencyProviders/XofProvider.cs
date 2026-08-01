using gemini.Interfaces;
using gemini.Models;
using gemini.Services.CurrencyParser;
using gemini.Services.HtmlProviders;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace gemini.Services.CurrencyProviders
{

    public class XofProvider : ICurrencyProvider
    {
        public CurrencyCode CurrencyCode => CurrencyCode.XOF;

        private const string baseUrl = "https://www.bceao.int/en/cours/get_all_devise_by_date";

        private readonly ISeleniumProvider _driverProvider;
        private readonly XOFParserService _parserService;

        public XofProvider(ISeleniumProvider driverProvider, XOFParserService parserService, ILogger<XofProvider> logger)
        {
            _driverProvider = driverProvider;
            _parserService = parserService;
        }

        public async Task<ExchangeRateRaw?> GetExchangeRate(DateOnly date, CancellationToken cancellationToken = default)
        {
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