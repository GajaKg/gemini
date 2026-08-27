using Microsoft.EntityFrameworkCore;
using Scrap.Domain.Entities;
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
            .ToListAsync(cancellationToken);
        // return await _context.Currencies
        //     .AsNoTracking()
        //     .Include(c => c.ExchangeRates
        //         .OrderByDescending(er => er.Date)
        //     )
        //         .ThenInclude(er => er.TargetCurrency)
        //     .ToListAsync(cancellationToken);
    }

}