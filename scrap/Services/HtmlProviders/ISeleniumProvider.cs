
using HtmlAgilityPack;

namespace gemini.Services.HtmlProviders
{
    public interface ISeleniumProvider
    {
        Task<HtmlDocument?> GetHtml(string url, CancellationToken cancellationToken = default);
    }
}