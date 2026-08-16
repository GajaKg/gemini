
using Scrap.Domain.Models;

namespace ScrapAPI.Repositories;

public interface ICurrencyRepository
{
    Task<IReadOnlyList<Currency>> GetAllAsync(CancellationToken cancellationToken);
    Task<Currency?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Currency?> GetByIdAndRateCurrencyIdAsync(int id, int rateCurrencyId, CancellationToken cancellationToken);
}