using System.ComponentModel.DataAnnotations;

namespace scrap.Dtos
{
    public class CurrencyRequest
    {
        [Required]
        public string Code { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
    }
}