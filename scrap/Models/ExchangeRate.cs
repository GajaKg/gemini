
using gemini.Interfaces;

namespace gemini.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        public Currency? Currency = null;
        public int TargetCurrencyId { get; set; }
        public Currency? TargetCurrency = null;
        public decimal Sell { get; set; }
        public decimal Buy { get; set; }
        public decimal Middle { get; set; }
        public DateOnly Date { set; get; }


        public string DetailInfo()
        {
            return $"Buy={Buy}, Sell={Sell}, Middle={Middle}";
        }
    }

    public class ExchangeRateRaw
    {
        public decimal Sell { get; set; }
        public decimal Buy { get; set; }
        public decimal Middle { get; set; }
        public CurrencyCode TargetCurrency { get; set; }
    }
}