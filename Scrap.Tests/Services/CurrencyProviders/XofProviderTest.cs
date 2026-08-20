using gemini.Services.CurrencyParser;
using gemini.Services.CurrencyProviders;
using gemini.Services.HtmlProviders;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Moq;
using Scrap.Domain.Enums;
using Scrap.Domain.Models;
using Xunit;

namespace Scrap.Tests.Services.CurrencyProviders;

public class XofProviderTests
{
    private readonly Mock<IHttpClientProvider> _httpClientProvider;
    private readonly Mock<IXofParserService> _parserService;
    private readonly Mock<ILogger<XofProvider>> _logger;

    private readonly XofProvider _provider;

    public XofProviderTests()
    {
        _httpClientProvider = new Mock<IHttpClientProvider>();
        _parserService = new Mock<IXofParserService>();
        _logger = new Mock<ILogger<XofProvider>>();

        _provider = new XofProvider(
            _httpClientProvider.Object,
            _parserService.Object,
            _logger.Object);
    }

    [Fact]
    public void CurrencyCode_ShouldBeXOF()
    {
        Assert.Equal(CurrencyCode.XOF, _provider.CurrencyCode);
    }

    [Fact]
    public async Task GetExchangeRate_ShouldRequestCorrectUrl()
    {
        // Arrange
        var date = new DateOnly(2025, 5, 7);

        var document = new HtmlDocument();

        _httpClientProvider
            .Setup(x => x.GetHtml(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(document);

        _parserService
            .Setup(x => x.Parse(document, CancellationToken.None))
            .ReturnsAsync([]);

        // Act
        await _provider.GetExchangeRate(date);

        // Assert
        _httpClientProvider.Verify(
            x => x.GetHtml(
                "https://www.bceao.int/en/cours/get_all_devise_by_date?dateJour=2025-5-7",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetExchangeRate_WhenHtmlIsNull_ShouldReturnNull()
    {
        // Arrange
        var date = new DateOnly(2025, 5, 7);

        _httpClientProvider
            .Setup(x => x.GetHtml(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((HtmlDocument?)null);

        // Act
        var result = await _provider.GetExchangeRate(date);

        // Assert
        Assert.Null(result);

        _parserService.Verify(
            x => x.Parse(It.IsAny<HtmlDocument>(), CancellationToken.None),
            Times.Never);
    }

    [Fact]
    public async Task GetExchangeRate_WhenHtmlExists_ShouldCallParser()
    {
        // Arrange
        var date = new DateOnly(2025, 5, 7);

        var document = new HtmlDocument();

        var expected = new List<ExchangeRateRaw>
        {
            new()
            {
                TargetCurrency = CurrencyCode.EUR,
                Buy = 655.957m,
                Sell = 655.957m,
                Middle = 655.957m
            }
        };

        _httpClientProvider
            .Setup(x => x.GetHtml(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(document);

        _parserService
            .Setup(x => x.Parse(document, CancellationToken.None))
            .ReturnsAsync(expected);

        // Act
        var result = await _provider.GetExchangeRate(date);

        // Assert
        Assert.Same(expected, result);

        _parserService.Verify(
            x => x.Parse(document, CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GetExchangeRate_ShouldPassCancellationToken()
    {
        // Arrange
        var date = new DateOnly(2025, 5, 7);
        using var cts = new CancellationTokenSource();

        var document = new HtmlDocument();

        _httpClientProvider
            .Setup(x => x.GetHtml(
                It.IsAny<string>(),
                cts.Token))
            .ReturnsAsync(document);

        _parserService
            .Setup(x => x.Parse(document, CancellationToken.None))
            .ReturnsAsync([]);

        // Act
        await _provider.GetExchangeRate(date, cts.Token);

        // Assert
        _httpClientProvider.Verify(
            x => x.GetHtml(
                It.IsAny<string>(),
                cts.Token),
            Times.Once);
    }
}