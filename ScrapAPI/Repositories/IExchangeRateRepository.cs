
using Scrap.Domain.Models;
using ScrapAPI.Helpers;

namespace ScrapAPI.Repositories;

public interface IExchangeRateRepository
{
    Task<PagedList<ExchangeRate>> GetRatesByCurrencySourceAndTargetId(ExchangeRateQueryParams rateQueryParams, CancellationToken cancellationToken);
}