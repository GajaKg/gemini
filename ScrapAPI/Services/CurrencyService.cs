using Scrap.Domain.Models;
using ScrapAPI.Dto;
using ScrapAPI.Mappers;
using ScrapAPI.Repositories;

namespace ScrapAPI.Services;

public class CurrencyService : ICurrencyService
{
    private readonly ICurrencyRepository _currencyRepository;

    public CurrencyService(ICurrencyRepository currencyRepository)
    {
        _currencyRepository = currencyRepository;
    }

    public async Task<IEnumerable<CurrencyDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var currencies = await _currencyRepository.GetAllAsync(cancellationToken);
        return currencies
            .Select(c => MapToDto(c))
            .ToList();
    }

    public async Task<CurrencyDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var currency = await _currencyRepository.GetByIdAsync(id, cancellationToken);

        if (currency is null) return null;

        return MapToDto(currency);
    }

    public async Task<CurrencyDto?> GetByIdAndRateCurrencyIdAsync(int id, int rateCurrencyId, CancellationToken cancellationToken)
    {
        var currency = await _currencyRepository.GetByIdAndRateCurrencyIdAsync(id, rateCurrencyId, cancellationToken);

        if (currency is null) return null;

        return MapToDto(currency);
    }

    private static CurrencyDto MapToDto(Currency currency)
    {
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
                    Currency = er.TargetCurrency?.ToCurrencyWithoutRatesDto()
                })
                .ToList()
        };
    }
}