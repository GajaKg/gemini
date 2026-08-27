using Scrap.Domain.Enums;

namespace Scrap.Domain.Models;
public class ExchangeRateRaw
{
    public decimal Sell { get; set; }
    public decimal Buy { get; set; }
    public decimal Middle { get; set; }
    public CurrencyCode TargetCurrency { get; set; }
}