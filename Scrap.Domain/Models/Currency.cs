using System.ComponentModel.DataAnnotations;
using Scrap.Domain.Enums;

namespace Scrap.Domain.Models;

public class Currency
{
    public int Id { get; set; }
    [Required]
    public CurrencyCode Code { get; set; }// MAD, XOF
    public string Name { get; set; } = null!; // Moroccan Dirham

    public ICollection<ExchangeRate> TargetExchangeRates { get; set; } = [];
    public ICollection<ExchangeRate> ExchangeRates { get; set; } = [];
    // public ICollection<ExchangeRate> SourceExchangeRates { get; set; } = [];
    // public ICollection<ExchangeRate> TargetExchangeRates { get; set; } = [];
}