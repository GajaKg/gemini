using gemini.Models;
using HtmlAgilityPack;

namespace gemini.Services.CurrencyParser
{
    public interface IParserService
    {
        public List<ExchangeRateRaw>? Parse(HtmlDocument doc);
    }

    public interface IXofParserService : IParserService;
    public interface IMadParserService : IParserService;
}