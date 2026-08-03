

using gemini.Data;
using gemini.Interfaces;
using gemini.Models;
using Microsoft.EntityFrameworkCore;

namespace gemini.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly ApplicationDBContext _context;

        public CurrencyRepository(ApplicationDBContext context)
        {
            _context = context;

        }
        public async Task<Currency?> GetCurrencyByCode(CurrencyCode code)
        {
            var currency = await _context.Currencies
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Code == code);

            if (currency is null) return null;

            return currency;
        }
    }
}