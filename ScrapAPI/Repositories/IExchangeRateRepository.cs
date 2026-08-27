
using Scrap.Domain.Entities;
using ScrapAPI.Helpers;

namespace ScrapAPI.Repositories;

public interface IExchangeRateRepository
{
    Task<PagedList<ExchangeRate>> GetRatesByCurrencySourceAndTargetId(ExchangeRateQueryParams rateQueryParams, CancellationToken cancellationToken);
}