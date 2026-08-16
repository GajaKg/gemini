using ScrapAPI.Dto;

namespace ScrapAPI.Services;

public interface ICurrencyService
{
    Task<IEnumerable<CurrencyDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<CurrencyDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<CurrencyDto?> GetByIdAndRateCurrencyIdAsync(int id, int rateCurrencyId, CancellationToken cancellationToken);
}