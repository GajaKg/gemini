using gemini.Models;
using HtmlAgilityPack;

namespace gemini.Services
{
    public interface IParserService
    {
        ExchangeRate? Parse(HtmlDocument doc, DateOnly date, int currencyId);
        // ExchangeRate? Parse(HtmlDocument doc);
    }
}