
using gemini.Interfaces;
using gemini.Services.CurrencyParser;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Scrap.Tests.CurrencyParser
{
    public class XOFParserServiceTest
    {
        [Fact]
        public void Parse_WhenHtmlContainsValidRates_ReturnsEuroAndUsdRates()
        {
            var parserService = new XOFParserService(NullLogger<XOFParserService>.Instance);
            var html = """
                <table>
                    <tbody>
                        <tr><td>Currency</td><td>Purchase</td><td>Sale</td></tr>
                        <tr><td>eUr</td><td>655,957</td><td>655,957</td></tr>
                        <tr><td>usD</td><td>587,000</td><td>594,000</td></tr>
                    </tbody>
                </table>
                """;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var result = parserService.Parse(doc);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(CurrencyCode.EUR, result[0].TargetCurrency);
            Assert.Equal(655.957m, result[0].Buy);
        }

        [Fact]
        public void Parse_WhenHtmlContainsInvalidRates_ReturnsNull()
        {
            // Arrange
            var parserService = new XOFParserService(NullLogger<XOFParserService>.Instance);
            var html = """
                <h1>No rates</h1>
                """;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var result = parserService.Parse(doc);

            Assert.Null(result);
        }
    }
}