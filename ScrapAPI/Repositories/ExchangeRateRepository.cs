using Microsoft.EntityFrameworkCore;
using Scrap.Domain.Models;
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

    public async Task<PagedList<ExchangeRate>> GetRatesByCurrencySourceAndTargetId(int id, int targetId, DateOnly? date, PaginationParams pagination, CancellationToken cancellationToken)
    {
        var sourceQuery = _context.ExchangeRates
            .AsNoTracking()
            .Where(er => er.CurrencyId == id && er.TargetCurrencyId == targetId);

        if (date.HasValue)
        {
            sourceQuery = sourceQuery.Where(er => er.Date == date.Value);
        }

        sourceQuery = sourceQuery.OrderByDescending(er => er.Date)
                .ThenByDescending(er => er.Id);

        return await PagedList<ExchangeRate>.CreateAsync(sourceQuery, pagination.CurrentPage, pagination.PageSize, cancellationToken);
    }
}