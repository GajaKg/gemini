using gemini.Services.CurrencyParser;
using HtmlAgilityPack;
using gemini.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;

namespace Scrap.Tests.CurrencyParser
{
    public class MADParserServiceTest
    {
        [Fact]
        public void Parse_WhenHtmlContainsValidRates_ReturnsEuroAndUsdRates()
        {
            // Arrange
            var parserService = new MADParserService(NullLogger<MADParserService>.Instance);
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

            var result = parserService.Parse(doc);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(CurrencyCode.EUR, result[0].TargetCurrency);
            Assert.Equal(10.3419m, result[0].Buy);
        }

        [Fact]
        public void Parse_WhenHtmlContainsInvalidRates_ReturnsNull()
        {
            // Arrange
            var parserService = new MADParserService(NullLogger<MADParserService>.Instance);
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