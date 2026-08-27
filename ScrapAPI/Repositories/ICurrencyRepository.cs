
using Scrap.Domain.Entities;

namespace ScrapAPI.Repositories;

public interface ICurrencyRepository
{
    Task<IReadOnlyList<Currency>> GetAllAsync(CancellationToken cancellationToken);
}