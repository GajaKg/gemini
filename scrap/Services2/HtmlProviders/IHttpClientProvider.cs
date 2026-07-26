using HtmlAgilityPack;

namespace gemini.Services.HtmlProviders
{
    public interface IHttpClientProvider
    {
        Task<HtmlDocument?> GetHtml(string url, CancellationToken cancellationToken = default);
    }
}