using gemini.Interfaces;
using gemini.Models;
using gemini.Services.CurrencyParser;
using gemini.Services.CurrencyProviders;
using gemini.Services.HtmlProviders;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OpenQA.Selenium;

namespace Scrap.Tests.CurrencyProviders;

public class MadProviderTests
{

    [Fact]
    public async Task GetExchangeRate_WhenHtmlExists_ReturnsRates()
    {
        // Arrange
        var seleniumProvider = new Mock<ISeleniumProvider>();
        var parserService = new Mock<IMadParserService>();

        var document = new HtmlDocument();

        var expectedRates = new List<ExchangeRateRaw>
        {
            new()
            {
                TargetCurrency = CurrencyCode.EUR,
                Buy = 655.957m,
                Sell = 655.957m,
                Middle = 655.957m
            },
            new()
            {
                TargetCurrency = CurrencyCode.USD,
                Buy = 584.250m,
                Sell = 591.250m,
                Middle = 587.750m
            }
        };

        seleniumProvider
            .Setup(x => x.GetHtml(
                It.IsAny<string>(),
                It.IsAny<By>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(document);

        parserService
            .Setup(x => x.Parse(document))
            .Returns(expectedRates);

        var provider = new MadProvider(
            seleniumProvider.Object,
            parserService.Object
        );

        var date = new DateOnly(2025, 1, 1);

        // Act
        var result = await provider.GetExchangeRate(
            date,
            CancellationToken.None
        );

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Equal(
            CurrencyCode.EUR,
            result[0].TargetCurrency
        );

        Assert.Equal(
            655.957m,
            result[0].Buy
        );

        Assert.Equal(
            CurrencyCode.USD,
            result[1].TargetCurrency
        );

        Assert.Equal(
            584.250m,
            result[1].Buy
        );

        seleniumProvider.Verify(
            x => x.GetHtml(
                It.Is<string>(url =>
                    url.Contains("1%2F1%2F2025")),
                It.Is<By>(by =>
                    by.ToString().Contains(".object_name")),
                It.IsAny<CancellationToken>()),
            Times.Once
        );

        parserService.Verify(
            x => x.Parse(document),
            Times.Once
        );
    }

    [Fact]
    public async Task GetExchangeRate_WhenHtmlIsNull_ReturnsNull()
    {
        // Arrange
        var seleniumProvider = new Mock<ISeleniumProvider>();
        var parserService = new Mock<IMadParserService>();

        seleniumProvider
            .Setup(x => x.GetHtml(
                It.IsAny<string>(),
                It.IsAny<By>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((HtmlDocument?)null);

        var provider = new MadProvider(
            seleniumProvider.Object,
            parserService.Object
        );

        // Act
        var result = await provider.GetExchangeRate(
            new DateOnly(2025, 1, 1),
            CancellationToken.None
        );

        // Assert
        Assert.Null(result);

        parserService.Verify(
            x => x.Parse(It.IsAny<HtmlDocument>()),
            Times.Never
        );
    }

    [Fact]
    public async Task GetExchangeRate_WhenParserReturnsRates_ReturnsSameRates()
    {
        // Arrange
        var seleniumProvider = new Mock<ISeleniumProvider>();
        var parserService = new Mock<IMadParserService>();

        var document = new HtmlDocument();

        var expectedRates = new List<ExchangeRateRaw>
    {
        new()
        {
            TargetCurrency = CurrencyCode.EUR,
            Buy = 100m,
            Sell = 110m,
            Middle = 105m
        }
    };

        seleniumProvider
            .Setup(x => x.GetHtml(
                It.IsAny<string>(),
                It.IsAny<By>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(document);

        parserService
            .Setup(x => x.Parse(document))
            .Returns(expectedRates);

        var provider = new MadProvider(
            seleniumProvider.Object,
            parserService.Object
        );

        // Act
        var result = await provider.GetExchangeRate(
            new DateOnly(2025, 1, 1)
        );

        // Assert
        Assert.Same(expectedRates, result);
    }
}