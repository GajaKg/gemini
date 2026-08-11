using ScrapAPI.Dto;

namespace ScrapAPI.Services;

public interface ICurrencyService
{
    Task<IEnumerable<CurrencyDto>> GetAllAsync();
    Task<IEnumerable<CurrencyDto>> GetAllTargetAsync();
    Task<CurrencyDto?> GetByIdAsync(int id);
    Task<CurrencyDto?> GetByIdTargetAsync(int id);
    Task<CurrencyDto?> GetByIdAndRateCurrencyIdAsync(int id, int rateCurrencyId);
}