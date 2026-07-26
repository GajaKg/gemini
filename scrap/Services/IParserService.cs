using gemini.Models;
using HtmlAgilityPack;

namespace gemini.Services
{
    public interface IParserService
    {
        // ExchangeRate? Parse(HtmlDocument doc, DateOnly date, int currencyId);
        ExchangeRateRaw? Parse(HtmlDocument doc);
    }
}