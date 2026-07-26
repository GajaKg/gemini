
using HtmlAgilityPack;

namespace gemini.Services.HtmlProviders
{
    public class HttpClientProvider : IHttpClientProvider
    {
        private readonly HttpClient _httpClient;

        public HttpClientProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HtmlDocument?> GetHtml(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                string html = await _httpClient.GetStringAsync(url, cancellationToken);

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
                Console.WriteLine($"Failed to download: {ex.Message}");
                return null;
            }
        }
    }
}