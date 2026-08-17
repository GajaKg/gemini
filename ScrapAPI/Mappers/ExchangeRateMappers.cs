using Scrap.Domain.Models;
using ScrapAPI.Dto;

namespace ScrapAPI.Mappers;

public static class ExchangeRateMappers
{
    public static ExchangeRateDto ToExchangeRateDto(this ExchangeRate exchangeRate)
    {
        return new ExchangeRateDto
        {
            Id = exchangeRate.Id,
            Sell = exchangeRate.Sell,
            Buy = exchangeRate.Buy,
            Middle = exchangeRate.Middle,
            Date = exchangeRate.Date,
        };
    }
}