using System.ComponentModel.DataAnnotations;
using Scrap.Domain.Enums;

namespace gemini.Dtos;

public class CurrencyRequest
{
    [Required]
    public CurrencyCode Code { get; set; }
    public string Name { get; set; } = String.Empty;
}