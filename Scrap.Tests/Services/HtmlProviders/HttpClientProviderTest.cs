using System.Net;
using gemini.Services.HtmlProviders;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;

namespace Scrap.Tests.HtmlProviders
{
    public class HttpClientProviderTest
    {
        [Fact]
        public async Task GetHtml_ReturnsDocumentWhenRequestSucceeds()
        {
            // Arrange
            const string html = "<html><body><h1>Currency data</h1></body></html>";

            var handler = new Mock<HttpMessageHandler>();
            handler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(html)
                });

            var httpClient = new HttpClient(handler.Object);
            
            var provider = new HttpClientProvider(
                httpClient,
                NullLogger<HttpClientProvider>.Instance);

            // Act
            var document = await provider.GetHtml("https://www.test.test/");

            // Assert
            Assert.NotNull(document);
            Assert.Equal("Currency data", document.DocumentNode.SelectSingleNode("//h1")?.InnerText);
        }
    }
}