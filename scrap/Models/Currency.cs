

namespace gemini.Models
{
    public class Currency
    {
        public int Id { get; set; }

        public string Code { get; set; } = null!; // MAD, XOF
        public string Name { get; set; } = null!; // Moroccan Dirham

        public readonly ICollection<ExchangeRate> ExchangeRates = [];
    }
}