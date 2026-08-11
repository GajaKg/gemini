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

    public async Task<IReadOnlyList<Currency>> GetAllAsync()
    {
        return await _context.Currencies
            .AsNoTracking()
            .Include(c => c.ExchangeRates
                .OrderByDescending(er => er.Date)
            )
                .ThenInclude(er => er.TargetCurrency)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Currency>> GetAllTargetAsync()
    {
        return await _context.Currencies
            .AsNoTracking()
            .Include(c => c.TargetExchangeRates
                .OrderByDescending(er => er.Date)
            )
                .ThenInclude(er => er.Currency)
            .ToListAsync();
    }

    public async Task<Currency?> GetByIdAsync(int id)
    {
        return await _context.Currencies
            .AsNoTracking()
            .Include(c => c.ExchangeRates
                .OrderByDescending(er => er.Date)
            )
                .ThenInclude(er => er.TargetCurrency)
            .SingleOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Currency?> GetByIdTargetAsync(int id)
    {
        return await _context.Currencies
            .AsNoTracking()
            .Include(c => c.TargetExchangeRates
                .OrderByDescending(er => er.Date)
            )
                .ThenInclude(er => er.Currency)
            .SingleOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Currency?> GetByIdAndRateCurrencyIdAsync(int id, int rateCurrencyId)
    {
        return await _context.Currencies
            .AsNoTracking()
            .Include(c => c.ExchangeRates
                .Where(er => er.TargetCurrencyId == rateCurrencyId)
                .OrderByDescending(er => er.Date)
            )
                .ThenInclude(er => er.TargetCurrency)
            .SingleOrDefaultAsync(c => c.Id == id);
    }
}