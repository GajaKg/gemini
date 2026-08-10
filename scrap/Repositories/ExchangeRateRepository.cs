using gemini.Data;
using gemini.Dtos;
using Microsoft.EntityFrameworkCore;
using Scrap.Domain.Models;

namespace gemini.Repositories
{
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        private readonly ApplicationDBContext _context;

        public ExchangeRateRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task BulkSaveAsync(List<ExchangeRate> items)
        {
            await _context.ExchangeRates.AddRangeAsync(items);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear(); // free memory for next 10
        }

        public async Task<IEnumerable<ExchangeRateLookup>> GetAllRatesDatesAsync(int currencyId)
        {
            return await _context.ExchangeRates
                .AsNoTracking()
                .Where(e => e.CurrencyId == currencyId)
                .Select(e => new ExchangeRateLookup
                {
                    Date = e.Date,
                    TargetCurrencyId = e.TargetCurrencyId,
                })
                .ToListAsync();
        }
    }
}