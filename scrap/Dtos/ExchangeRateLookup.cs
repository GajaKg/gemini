namespace gemini.Dtos;

public record ExchangeRateLookup
{
    public DateOnly Date { get; init; }
    public int TargetCurrencyId { get; init; }
}