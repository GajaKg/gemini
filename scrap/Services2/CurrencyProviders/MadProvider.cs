using gemini.Interfaces;
using gemini.Models;
using gemini.Services.MAD;
using gemini.Services2.HtmlProviders;
using HtmlAgilityPack;

namespace gemini.Services2.CurrencyProviders
{

    public class MadProvider : ICurrencyProvider
    {
        public CurrencyCode CurrencyCode => CurrencyCode.MAD;

        private const string baseUrl = "https://www.bkam.ma/en/Markets/Key-indicators/Foreign-exchange-market/Foreign-exchange-rates/Foreign-banknotes-exchange-rate";

        private readonly ISeleniumProvider _seleniumProvider;
        private readonly MADParserService _parserService;

        public MadProvider(ISeleniumProvider seleniumProvider, MADParserService parserService)
        {
            _seleniumProvider = seleniumProvider;
            _parserService = parserService;
        }

        public async Task<ExchangeRateRaw?> GetExchangeRate(DateOnly date, CancellationToken cancellationToken = default)
        {
            int delay = Random.Shared.Next(4000, 10001);
            await Task.Delay(delay, cancellationToken);

            string urlByDay = $"{baseUrl}?date={date.Day}%2F{date.Month}%2F{date.Year}&block=d1f170603d8b478a6a7b3447ae7f68f3#address-c2e03d492b315ebd7817808fde6acc08-d1f170603d8b478a6a7b3447ae7f68f3";

            // get html
            HtmlDocument? doc = await _seleniumProvider.GetHtml(urlByDay, cancellationToken);
            if (doc is null) return null;

            // extracting data from html
            return _parserService.Parse(doc);
        }
    }
}