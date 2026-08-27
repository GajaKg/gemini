
using Scrap.Domain.Entities;
using ScrapAPI.Dto;

namespace ScrapAPI.Mappers;

public static class CurrencyMappers
{
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