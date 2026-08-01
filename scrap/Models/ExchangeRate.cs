
namespace gemini.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        public decimal Sell { get; set; }
        public decimal Buy { get; set; }
        public decimal Middle { get; set; }
        public DateOnly Date { set; get; }

        public Currency? Currency = null;

        public string DetailInfo()
        {
            return $"{CurrencyId}: Buy={Buy}, Sell={Sell}, Middle={Middle}";
        }
    }

    public class ExchangeRateRaw
    {
        public decimal Sell { get; set; }
        public decimal Buy { get; set; }
        public decimal Middle { get; set; }
    }
}