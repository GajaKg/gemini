
using Scrap.Domain.Models;
using ScrapAPI.Dto;

namespace ScrapAPI.Mappers;

public static class CurrencyMappers
{
    public static CurrencyDto ToCurrencyDto(this Currency currency)
    {
        return new CurrencyDto
        {
            Id = currency.Id,
            Code = currency.Code,
            Name = currency.Name,
            ExchangeRates = currency.ExchangeRates
                .Select(er => er.ToExchangeRateDto())
                .ToList(),
        };
    }
    public static CurrencyWithoutRatesDto ToCurrencyWithoutRatesDto(this Currency currency)
    {
        return new CurrencyWithoutRatesDto
        {
            Id = currency.Id,
            Code = currency.Code,
            Name = currency.Name
        };
    }
}