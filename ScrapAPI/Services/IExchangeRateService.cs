using ScrapAPI.Dto;
using ScrapAPI.Helpers;

namespace ScrapAPI.Services;

public interface IExchangeRateService
{
    Task<PagedList<ExchangeRateDto>> GetRatesByCurrencySourceAndTargetId(int id, int targetId, PaginationParams pagination, CancellationToken cancellationToken);
}