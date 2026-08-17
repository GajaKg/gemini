using HtmlAgilityPack;
using Scrap.Domain.Models;

namespace gemini.Services.CurrencyParser;

public interface IParserService
{
    public Task<List<ExchangeRateRaw>?> Parse(HtmlDocument doc, CancellationToken cancellationToken);
}

public interface IXofParserService : IParserService;
public interface IMadParserService : IParserService;
