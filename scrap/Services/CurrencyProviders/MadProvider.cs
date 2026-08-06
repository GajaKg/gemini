using gemini.Interfaces;
using gemini.Models;
using gemini.Services.CurrencyParser;
using gemini.Services.HtmlProviders;
using HtmlAgilityPack;
using OpenQA.Selenium;

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

        // <h2>Exchange rates of friday 3 january 2020</h2>
        // <table>
        //   <tbody>
        //     <tr> <th>Currency</th> <th>Purchase</th> <th>Sale</th> </tr>
        //     <tr> <td>EUR</td>   <td>655,957</td>   <td>655,957</td></tr>
        //     <tr> <td>USD</td>   <td>584,250</td>   <td>591,250</td> </tr>
        //     <tr> <td>JPY</td>   <td>5,410</td>   <td>5,470</td> </tr>
        //   </tbody>
        // </table>

        /// <summary>
        /// Get exchange rates
        /// </summary>
        /// <param name="date">Used in url to get rates at date</param>
        public async Task<List<ExchangeRateRaw>?> GetExchangeRate(DateOnly date, CancellationToken cancellationToken = default)
        {
            // Creates random delay to avoid scraping detection
            int delay = Random.Shared.Next(4000, 10001);
            await Task.Delay(delay, cancellationToken);

            string urlByDay = $"{baseUrl}?date={date.Day}%2F{date.Month}%2F{date.Year}&block=d1f170603d8b478a6a7b3447ae7f68f3#address-c2e03d492b315ebd7817808fde6acc08-d1f170603d8b478a6a7b3447ae7f68f3";

            // get html
            HtmlDocument? doc = await _driverProvider.GetHtml(
                urlByDay,
                By.CssSelector(".object_name"),
                cancellationToken
            );

            if (doc is null) return null;

            // extracting data from html
            return _parserService.Parse(doc);
        }
    }
}