using HtmlAgilityPack;
using Scrap.Domain.Models;

namespace gemini.Services.CurrencyParser;

public interface IParserService
{
    public List<ExchangeRateRaw>? Parse(HtmlDocument doc);
}

public interface IXofParserService : IParserService;
public interface IMadParserService : IParserService;