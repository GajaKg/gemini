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
            .Include(c => c.ExchangeRates)
                .ThenInclude(er => er.TargetCurrency)
            .ToListAsync();
    }
    
    public async Task<IReadOnlyList<Currency>> GetAllTargetAsync()
    {
        return await _context.Currencies
            .AsNoTracking()
            .Include(c => c.TargetExchangeRates)
                .ThenInclude(er => er.Currency)
            .ToListAsync();
    }
}