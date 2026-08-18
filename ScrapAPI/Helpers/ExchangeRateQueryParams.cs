namespace ScrapAPI.Helpers;

public class ExchangeRateQueryParamsSingle
{
    public int Id { get; set; }
    public int TargetCyrrencyId { get; set; }
    public DateOnly Date { get; set; }
}

public class ExchangeRateQueryParams : PaginationParams
{
    public int Id { get; set; }
    public int TargetCurrencyId { get; set; }
    public DateOnly? Date { get; set; }
}