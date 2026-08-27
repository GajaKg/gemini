

using Scrap.Domain.Enums;
using Scrap.Domain.Entities;

namespace gemini.Repositories
{
    public interface ICurrencyRepository
    {
        Task<Currency?> GetCurrencyByCode(CurrencyCode code, CancellationToken cancellationToken);
        Task<IEnumerable<Currency>> GetAllAsync(CancellationToken cancellationToken);
    }
}