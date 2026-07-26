
using HtmlAgilityPack;

namespace gemini.Services2.HtmlProviders
{
    public interface ISeleniumProvider
    {
        Task<HtmlDocument?> GetHtml(string url, CancellationToken cancellationToken = default);
    }
}