using gemini.Services.CurrencyParser;
using gemini.Services.Email;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Scrap.Domain.Enums;

namespace Scrap.Tests.Services.CurrencyParser
{
    public class MADParserServiceTest
    {
        [Fact]
        public async Task Parse_WhenHtmlContainsValidRates_ReturnsEuroAndUsdRates()
        {
            var emailService = new Mock<IEmailService>();
            var configuration = new Mock<IConfiguration>();

            var parserService = new MADParserService(
                NullLogger<MADParserService>.Instance,
                emailService.Object,
                configuration.Object
            );

            var html = """
                <table>
                    <tbody>
                        <tr>
                            <td><span class="object_name">1 EURO</span><br /></td>
                            <td>
                                <span class="number">10.3419&nbsp;<span class="symbol"></span></span>
                            </td>
                            <td>
                                <span class="number">11.4305&nbsp;<span class="symbol"></span></span>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span class="object_name">1 US DOLLAR</span><br />
                            </td>
                            <td>
                                <span class="number">9.26690&nbsp;<span class="symbol"></span></span>
                            </td>
                            <td>
                                <span class="number">10.2423&nbsp;<span class="symbol"></span></span>
                            </td>
                        </tr>
                    </tbody>
                </table>
                """;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var result = await parserService.Parse(doc, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(CurrencyCode.EUR, result[0].TargetCurrency);
            Assert.Equal(10.3419m, result[0].Buy);
        }

        [Fact]
        public async Task Parse_WhenHtmlContainsInvalidRates_ReturnsNull()
        {
            // Arrange
            var emailService = new Mock<IEmailService>();
            var configuration = new Mock<IConfiguration>();

            var parserService = new MADParserService(
                NullLogger<MADParserService>.Instance,
                emailService.Object,
                configuration.Object
            );

            var html = """
                <h1>No rates</h1>
                """;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var result = await parserService.Parse(doc, CancellationToken.None);

            Assert.Null(result);
        }


        [Fact]
        public async Task Parse_WhenHtmlStructureChanges_SendsCriticalErrorEmail()
        {
            // Arrange
            var emailService = new Mock<IEmailService>();
            var configuration = new Mock<IConfiguration>();

            configuration
                .Setup(c => c["Email:ErrorReciever"])
                .Returns("test@example.com");

            var parserService = new MADParserService(
                NullLogger<MADParserService>.Instance,
                emailService.Object,
                configuration.Object
            );

            var html = """
            <h1>Something completely changed</h1>
            """;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Act
            var result = await parserService.Parse(
                doc,
                CancellationToken.None
            );

            // Assert
            Assert.Null(result);

            emailService.Verify(
                e => e.ComposeMessage(
                    "test@example.com",
                    "MAD scraper critical error",
                    "Please check page for html changes!",
                    It.IsAny<CancellationToken>()),
                Times.Once
            );
        }


        [Fact]
        public async Task Parse_WhenErrorReceiverIsMissing_DoesNotSendEmail()
        {
            // Arrange
            var emailService = new Mock<IEmailService>();
            var configuration = new Mock<IConfiguration>();

            configuration
                .Setup(c => c["Email:ErrorReciever"])
                .Returns((string?)null);

            var parserService = new MADParserService(
                NullLogger<MADParserService>.Instance,
                emailService.Object,
                configuration.Object
            );

            var html = """
            <h1>Something completely changed</h1>
            """;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Act
            var result = await parserService.Parse(
                doc,
                CancellationToken.None
            );

            // Assert
            Assert.Null(result);

            emailService.Verify(
                e => e.ComposeMessage(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never
            );
        }


        [Fact]
        public async Task Parse_WhenEmailIsCancelled_PropagatesCancellation()
        {
            // Arrange
            var emailService = new Mock<IEmailService>();
            var configuration = new Mock<IConfiguration>();

            configuration
                .Setup(c => c["Email:ErrorReciever"])
                .Returns("test@example.com");

            emailService
                .Setup(e => e.ComposeMessage(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            var parserService = new MADParserService(
                NullLogger<MADParserService>.Instance,
                emailService.Object,
                configuration.Object
            );

            var html = """
            <h1>Something completely changed</h1>
            """;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Act + Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                parserService.Parse(
                    doc,
                    CancellationToken.None
                )
            );
        }
    }
}