
namespace ScrapAPI.Dto;

public class ExchangeRateDto
{
    public int Id { get; set; }
    public decimal Sell { get; set; }
    public decimal Buy { get; set; }
    public decimal Middle { get; set; }
    public DateOnly Date { set; get; }
    // public CurrencyWithoutRatesDto? TargetCurrency { get; set; }
    // public CurrencyWithoutRatesDto? Currency { get; set; }
}