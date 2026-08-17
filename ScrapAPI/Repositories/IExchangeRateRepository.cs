
using Scrap.Domain.Models;
using ScrapAPI.Helpers;

namespace ScrapAPI.Repositories;

public interface IExchangeRateRepository
{
    Task<PagedList<ExchangeRate>> GetRatesByCurrencySourceAndTargetId(int id, int targetId, PaginationParams pagination, CancellationToken cancellationToken);
}