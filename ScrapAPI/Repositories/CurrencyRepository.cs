using Microsoft.EntityFrameworkCore;
using Scrap.Domain.Models;
using ScrapAPI.Data;

namespace ScrapAPI.Repositories;

public class CurrencyRepository : ICurrencyRepository
{
    private readonly ApplicationDBContext _context;

    public CurrencyRepository(ApplicationDBContext dBContext)
    {
        _context = dBContext;
    }

    public async Task<IReadOnlyList<Currency>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Currencies
            .AsNoTracking()
            .Include(c => c.ExchangeRates
                .OrderByDescending(er => er.Date)
            )
                .ThenInclude(er => er.TargetCurrency)
            .ToListAsync(cancellationToken);
    }

    public async Task<Currency?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Currencies
            .AsNoTracking()
            .Include(c => c.ExchangeRates
                .OrderByDescending(er => er.Date)
            )
                .ThenInclude(er => er.TargetCurrency)
            .SingleOrDefaultAsync(c => c.Id == id, cancellationToken: cancellationToken);
    }

    public async Task<Currency?> GetByIdAndRateCurrencyIdAsync(int id, int rateCurrencyId, CancellationToken cancellationToken)
    {
        return await _context.Currencies
            .AsNoTracking()
            .Include(c => c.ExchangeRates
                .Where(er => er.TargetCurrencyId == rateCurrencyId)
                .OrderByDescending(er => er.Date)
            )
                .ThenInclude(er => er.TargetCurrency)
            .SingleOrDefaultAsync(c => c.Id == id, cancellationToken: cancellationToken);
    }
}