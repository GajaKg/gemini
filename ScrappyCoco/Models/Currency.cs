using ScrappyCoco.Enums;

namespace ScrappyCoco.Models;

public class Currency
{
    public int Id { get; set; }
    public CurrencyCode Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ExchangeRate> ExchangeRates {get; set;} = [];
}