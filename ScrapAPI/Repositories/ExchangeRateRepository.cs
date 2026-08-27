using Microsoft.EntityFrameworkCore;
using Scrap.Domain.Entities;
using ScrapAPI.Data;
using ScrapAPI.Helpers;

namespace ScrapAPI.Repositories;

public class ExchangeRateRepository : IExchangeRateRepository
{
    private readonly ApplicationDBContext _context;

    public ExchangeRateRepository(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<PagedList<ExchangeRate>> GetRatesByCurrencySourceAndTargetId(ExchangeRateQueryParams rateQueryParams, CancellationToken cancellationToken)
    {
        IQueryable<ExchangeRate> sourceQuery = _context.ExchangeRates
            .AsNoTracking()
            .Where(er => er.CurrencyId == rateQueryParams.Id && er.TargetCurrencyId == rateQueryParams.TargetCurrencyId);

        if (rateQueryParams.Date.HasValue)
        {
            sourceQuery = sourceQuery.Where(er => er.Date == rateQueryParams.Date.Value);
        }

        sourceQuery = sourceQuery.OrderByDescending(er => er.Date)
                .ThenByDescending(er => er.Id);

        return await PagedList<ExchangeRate>.CreateAsync(sourceQuery, rateQueryParams.CurrentPage, rateQueryParams.PageSize, cancellationToken);
    }
}