using System.ComponentModel.DataAnnotations;
using gemini.Interfaces;

namespace scrap.Dtos
{
    public class CurrencyRequest
    {
        [Required]
        public CurrencyCode Code { get; set; }
        public string Name { get; set; } = String.Empty;
    }
}