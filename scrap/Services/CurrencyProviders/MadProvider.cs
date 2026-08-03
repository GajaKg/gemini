using gemini.Interfaces;
using gemini.Models;
using gemini.Services.CurrencyParser;
using gemini.Services.HtmlProviders;
using HtmlAgilityPack;

namespace gemini.Services.CurrencyProviders
{

    public class MadProvider : ICurrencyProvider
    {
        public CurrencyCode CurrencyCode => CurrencyCode.MAD;

        private const string baseUrl = "https://www.bkam.ma/en/Markets/Key-indicators/Foreign-exchange-market/Foreign-exchange-rates/Foreign-banknotes-exchange-rate";

        private readonly ISeleniumProvider _driverProvider;
        private readonly MADParserService _parserService;

        public MadProvider(ISeleniumProvider driverProvider, MADParserService parserService)
        {
            _driverProvider = driverProvider;
            _parserService = parserService;
        }

        public async Task<List<ExchangeRateRaw>?> GetExchangeRate(DateOnly date, CancellationToken cancellationToken = default)
        {
            int delay = Random.Shared.Next(4000, 10001);
            await Task.Delay(delay, cancellationToken);

            string urlByDay = $"{baseUrl}?date={date.Day}%2F{date.Month}%2F{date.Year}&block=d1f170603d8b478a6a7b3447ae7f68f3#address-c2e03d492b315ebd7817808fde6acc08-d1f170603d8b478a6a7b3447ae7f68f3";

            // get html
            HtmlDocument? doc = await _driverProvider.GetHtml(urlByDay, cancellationToken);
            if (doc is null) return null;

            // extracting data from html
            return [];
            // return _parserService.Parse(doc);
        }
    }
}