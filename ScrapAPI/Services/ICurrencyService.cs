using Scrap.Domain.Models;
using ScrapAPI.Dto;

namespace ScrapAPI.Services;

public interface ICurrencyService
{
    Task<IEnumerable<CurrencyDto>> GetAllAsync();
    Task<IEnumerable<CurrencyDto>> GetAllTargetAsync();
}