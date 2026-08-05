
using HtmlAgilityPack;
using OpenQA.Selenium;

namespace gemini.Services.HtmlProviders
{
    public interface ISeleniumProvider
    {
        Task<HtmlDocument?> GetHtml(string url, By? waitElements, CancellationToken cancellationToken = default);
        Task<HtmlDocument?> GetHtml(string url, CancellationToken cancellationToken = default);
    }
}