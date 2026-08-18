using ScrapAPI.Dto;
using ScrapAPI.Helpers;

namespace ScrapAPI.Services;

public interface IExchangeRateService
{
    Task<PagedList<ExchangeRateDto>> GetRatesByCurrencySourceAndTargetId(int id, int targetId, DateOnly? date, PaginationParams pagination, CancellationToken cancellationToken);
}