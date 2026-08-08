using gemini.Dtos;
using gemini.Interfaces;
using gemini.Models;
using gemini.Repositories;
using gemini.Services;
using gemini.Services.CurrencyProviders;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Scrap.Tests.Services
{
    public class ExchangeRateScraperTest
    {
        private readonly Mock<ICurrencyProvider> _currencyProvider;
        private readonly Mock<ICurrencyRepository> _currencyRepository;
        private readonly Mock<IExchangeRateRepository> _exchangeRateRepository;

        private readonly Currency _mad;
        private readonly Currency _eur;
        private readonly Currency _usd;

        public ExchangeRateScraperTest()
        {
            _currencyProvider = new Mock<ICurrencyProvider>();
            _currencyRepository = new Mock<ICurrencyRepository>();
            _exchangeRateRepository = new Mock<IExchangeRateRepository>();

            _mad = new Currency
            {
                Id = 1,
                Code = CurrencyCode.MAD,
                Name = "Moroccan Dirham"
            };

            _eur = new Currency
            {
                Id = 2,
                Code = CurrencyCode.EUR,
                Name = "Euro"
            };

            _usd = new Currency
            {
                Id = 3,
                Code = CurrencyCode.USD,
                Name = "US Dollar"
            };

            _currencyProvider
                .Setup(x => x.CurrencyCode)
                .Returns(CurrencyCode.MAD);
        }

        private ExchangeRateScraper CreateScraper()
        {
            return new ExchangeRateScraper(
                _currencyProvider.Object,
                _currencyRepository.Object,
                _exchangeRateRepository.Object,
                NullLogger<ExchangeRateScraper>.Instance
            );
        }

        [Fact]
        public async Task Scrape_WhenSourceCurrencyDoesNotExist_DoesNotSaveRates()
        {
            // Arrange
            _currencyRepository
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var scraper = CreateScraper();

            // Act
            await scraper.Scrape(
                new DateOnly(2025, 1, 1),
                new DateOnly(2025, 1, 1)
            );

            // Assert
            _exchangeRateRepository.Verify(
                x => x.BulkSaveAsync(It.IsAny<List<ExchangeRate>>()),
                Times.Never
            );

            _currencyProvider.Verify(
                x => x.GetExchangeRate(
                    It.IsAny<DateOnly>(),
                    It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Scrape_WhenProviderReturnsNoRates_DoesNotSave()
        {
            // Arrange
            SetupCurrencies();

            _exchangeRateRepository
                .Setup(x => x.GetAllRatesDatesAsync(_mad.Id))
                .ReturnsAsync([]);

            _currencyProvider
                .Setup(x => x.GetExchangeRate(
                    It.IsAny<DateOnly>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var scraper = CreateScraper();

            // Act
            await scraper.Scrape(
                new DateOnly(2025, 1, 1),
                new DateOnly(2025, 1, 1)
            );

            // Assert
            _exchangeRateRepository.Verify(
                x => x.BulkSaveAsync(It.IsAny<List<ExchangeRate>>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Scrape_WhenRateAlreadyExists_DoesNotSaveDuplicate()
        {
            // Arrange
            SetupCurrencies();

            var date = new DateOnly(2025, 1, 1);

            _exchangeRateRepository
                .Setup(x => x.GetAllRatesDatesAsync(_mad.Id))
                .ReturnsAsync([
                    new ExchangeRateLookup
            {
                Date = date,
                TargetCurrencyId = _eur.Id
            }
                ]);

            _currencyProvider
                .Setup(x => x.GetExchangeRate(
                    date,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([
                    new ExchangeRateRaw
            {
                TargetCurrency = CurrencyCode.EUR,
                Buy = 10m,
                Sell = 11m,
                Middle = 10.5m
            }
                ]);

            var scraper = CreateScraper();

            // Act
            await scraper.Scrape(date, date);

            // Assert
            _exchangeRateRepository.Verify(
                x => x.BulkSaveAsync(It.IsAny<List<ExchangeRate>>()),
                Times.Never
            );
        }

        private void SetupCurrencies()
        {
            _currencyRepository
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync([
                    _mad,
            _eur,
            _usd
                ]);
        }
    }
}