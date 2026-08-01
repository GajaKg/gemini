using gemini.Models;
using HtmlAgilityPack;

namespace gemini.Services
{
    public interface IParserService
    {
        ExchangeRateRaw? Parse(HtmlDocument doc);
    }
}