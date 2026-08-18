
using Scrap.Domain.Models;

namespace ScrapAPI.Repositories;

public interface ICurrencyRepository
{
    Task<IReadOnlyList<Currency>> GetAllAsync(CancellationToken cancellationToken);
}