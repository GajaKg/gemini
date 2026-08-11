using ScrapAPI.Dto;
using ScrapAPI.Mappers;
using ScrapAPI.Repositories;

namespace ScrapAPI.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepository;

        public CurrencyService(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        public async Task<IEnumerable<CurrencyDto>> GetAllAsync()
        {
            var currencies = await _currencyRepository.GetAllAsync();
            return currencies
                .Select(c => new CurrencyDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    ExchangeRates = c.ExchangeRates
                        .Select(er => new ExchangeRateDto
                        {
                            Id = er.Id,
                            Sell = er.Sell,
                            Buy = er.Buy,
                            Middle = er.Middle,
                            Date = er.Date,
                            // TargetCurrency = er.TargetCurrency is null
                            Currency = er.TargetCurrency is null
                                ? null
                                : er.TargetCurrency!.ToCurrencyWithoutRatesDto(),
                        })
                        .ToList()
                })
                .ToList();
        }
        public async Task<IEnumerable<CurrencyDto>> GetAllTargetAsync()
        {
            var currencies = await _currencyRepository.GetAllTargetAsync();
            return currencies
                .Select(c => new CurrencyDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    ExchangeRates = c.TargetExchangeRates
                        .Select(er => new ExchangeRateDto
                        {
                            Id = er.Id,
                            Sell = er.Sell,
                            Buy = er.Buy,
                            Middle = er.Middle,
                            Date = er.Date,
                            Currency = er.Currency is null
                                ? null
                                : er.Currency!.ToCurrencyWithoutRatesDto(),
                        })
                        .ToList()
                })
                .ToList();
        }


        public async Task<CurrencyDto?> GetByIdAsync(int id)
        {
            var currency = await _currencyRepository.GetByIdAsync(id);

            if (currency is null) return null;

            return new CurrencyDto
            {
                Id = currency.Id,
                Code = currency.Code,
                Name = currency.Name,
                ExchangeRates = currency.ExchangeRates
                                    .Select(er => new ExchangeRateDto
                                    {
                                        Id = er.Id,
                                        Sell = er.Sell,
                                        Buy = er.Buy,
                                        Middle = er.Middle,
                                        Date = er.Date,
                                        Currency = er.TargetCurrency is null
                                            ? null
                                            : er.TargetCurrency!.ToCurrencyWithoutRatesDto(),
                                    })
                                    .ToList()
            };
        }

        public async Task<CurrencyDto?> GetByIdTargetAsync(int id)
        {
            var currency = await _currencyRepository.GetByIdTargetAsync(id);

            if (currency is null) return null;

            return new CurrencyDto
            {
                Id = currency.Id,
                Code = currency.Code,
                Name = currency.Name,
                ExchangeRates = currency.TargetExchangeRates
                                    .Select(er => new ExchangeRateDto
                                    {
                                        Id = er.Id,
                                        Sell = er.Sell,
                                        Buy = er.Buy,
                                        Middle = er.Middle,
                                        Date = er.Date,
                                        Currency = er.Currency is null
                                            ? null
                                            : er.Currency!.ToCurrencyWithoutRatesDto(),
                                    })
                                    .ToList()
            };
        }

        public async Task<CurrencyDto?> GetByIdAndRateCurrencyIdAsync(int id, int rateCurrencyId)
        {
            var currency = await _currencyRepository.GetByIdAndRateCurrencyIdAsync(id, rateCurrencyId);

            if (currency is null) return null;

            return new CurrencyDto
            {
                Id = currency.Id,
                Code = currency.Code,
                Name = currency.Name,
                ExchangeRates = currency.ExchangeRates
                                    .Select(er => new ExchangeRateDto
                                    {
                                        Id = er.Id,
                                        Sell = er.Sell,
                                        Buy = er.Buy,
                                        Middle = er.Middle,
                                        Date = er.Date,
                                        Currency = er.TargetCurrency is null
                                            ? null
                                            : er.TargetCurrency!.ToCurrencyWithoutRatesDto(),
                                    })
                                    .ToList()
            };
        }
    }
}