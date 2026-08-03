using gemini.Models;
using HtmlAgilityPack;

namespace gemini.Services
{
    public interface IParserService
    {
        public List<ExchangeRateRaw>? Parse(HtmlDocument doc);
    }
}