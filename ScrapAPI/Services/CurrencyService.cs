using ScrapAPI.Dto;
using ScrapAPI.Mappers;
using ScrapAPI.Repositories;

namespace ScrapAPI.Services;

public class CurrencyService : ICurrencyService
{
    private readonly ICurrencyRepository _currencyRepository;

    public CurrencyService(ICurrencyRepository currencyRepository)
    {
        _currencyRepository = currencyRepository;
    }

    public async Task<IEnumerable<CurrencyWithoutRatesDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var currencies = await _currencyRepository.GetAllAsync(cancellationToken);
        return currencies
            .Select(c => c.ToCurrencyWithoutRatesDto())
            .ToList();
    }

}