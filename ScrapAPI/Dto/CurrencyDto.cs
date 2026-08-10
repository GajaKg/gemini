
using System.ComponentModel.DataAnnotations;
using Scrap.Domain.Enums;

namespace ScrapAPI.Dto;

public class CurrencyDto
{
    public int Id { get; set; }
    [Required]
    public CurrencyCode Code { get; set; }// MAD, XOF
    public string Name { get; set; } = null!; // Moroccan Dirham

    public IReadOnlyList<ExchangeRateDto> ExchangeRates { get; set; } = [];
}


public class CurrencyWithoutRatesDto
{
    public int Id { get; set; }
    [Required]
    public CurrencyCode Code { get; set; }// MAD, XOF
    public string Name { get; set; } = null!; // Moroccan Dirham
}