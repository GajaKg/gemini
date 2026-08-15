
namespace ScrappyCoco.Models;

public class ExchangeRate
{
    public int Id { get; set; }
    public decimal Buy { get; set; }
    public decimal Sell { get; set; }
    public decimal Middle { get; set; }
    public DateOnly Date { get; set; }
    public required Currency Currency { get; set; }
}