
using Scrap.Domain.Models;

namespace ScrapAPI.Repositories;

public interface ICurrencyRepository
{
    Task<IReadOnlyList<Currency>> GetAllAsync();
    Task<IReadOnlyList<Currency>> GetAllTargetAsync();
    Task<Currency?> GetByIdAsync(int id);
    Task<Currency?> GetByIdTargetAsync(int id);
    Task<Currency?> GetByIdAndRateCurrencyIdAsync(int id, int rateCurrencyId);
}