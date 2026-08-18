using ScrapAPI.Dto;
using ScrapAPI.Helpers;

namespace ScrapAPI.Services;

public interface IExchangeRateService
{
    Task<PagedList<ExchangeRateDto>> GetRatesByCurrencySourceAndTargetId(ExchangeRateQueryParams rateQueryParams, CancellationToken cancellationToken);
}