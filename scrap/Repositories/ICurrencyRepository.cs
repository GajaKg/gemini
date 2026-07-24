
using gemini.Interfaces;
using gemini.Models;

namespace gemini.Repositories
{
    public interface ICurrencyRepository
    {
        Task<Currency?> GetCurrencyByCode(CurrencyCode code);
    }
}