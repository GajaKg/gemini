using gemini.Models;
using HtmlAgilityPack;

namespace gemini.Services
{
    public interface IParserService
    {
        Task<ExchangeRate?> Parse(HtmlDocument doc, DateOnly date);
    }
}