

using gemini.Data;
using Microsoft.EntityFrameworkCore;
using Scrap.Domain.Interfaces;
using Scrap.Domain.Models;

namespace gemini.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly ApplicationDBContext _context;

        public CurrencyRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Currency>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Currencies
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Currency?> GetCurrencyByCode(CurrencyCode code, CancellationToken cancellationToken)
        {
            var currency = await _context.Currencies
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Code == code, cancellationToken: cancellationToken);

            if (currency is null) return null;

            return currency;
        }
    }
}