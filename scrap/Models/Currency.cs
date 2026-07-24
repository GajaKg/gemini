

using System.ComponentModel.DataAnnotations;
using gemini.Interfaces;

namespace gemini.Models
{
    public class Currency
    {
        public int Id { get; set; }
        [Required]
        public string Code { get; set; } // MAD, XOF
        public string Name { get; set; } = null!; // Moroccan Dirham

        public readonly ICollection<ExchangeRate> ExchangeRates = [];
    }
}