
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace gemini.Services.HtmlProviders
{
    public class HttpClientProvider : IHttpClientProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpClientProvider> _logger;

        public HttpClientProvider(HttpClient httpClient, ILogger<HttpClientProvider> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<HtmlDocument?> GetHtml(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                string html = await _httpClient.GetStringAsync(url + "asd", cancellationToken);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                return doc;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "❌ Failed request");
                return null;
            }
        }
    }
}