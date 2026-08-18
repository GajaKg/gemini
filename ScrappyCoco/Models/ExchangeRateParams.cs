namespace ScrappyCoco.Models;

public class ExchangeRateParams : PaginationParams
{
    public int CurrencyForId { set; get; }
    public int CurrencyTargetId { set; get; }
    public DateTime? SearchDate { set; get; }
}