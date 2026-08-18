using ScrapAPI.Dto;

namespace ScrapAPI.Services;

public interface ICurrencyService
{
    Task<IEnumerable<CurrencyWithoutRatesDto>> GetAllAsync(CancellationToken cancellationToken);
}