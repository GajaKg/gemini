
using ScrapAPI.Dto;
using ScrapAPI.Helpers;
using ScrapAPI.Mappers;
using ScrapAPI.Repositories;

namespace ScrapAPI.Services;

public class ExchangeRateService : IExchangeRateService
{
    private readonly IExchangeRateRepository _exchangeRateRepository;

    public ExchangeRateService(IExchangeRateRepository exchangeRateRepository)
    {
        _exchangeRateRepository = exchangeRateRepository;
    }

    public async Task<PagedList<ExchangeRateDto>> GetRatesByCurrencySourceAndTargetId(ExchangeRateQueryParams rateQueryParams, CancellationToken cancellationToken)
    {
        var rates = await _exchangeRateRepository.GetRatesByCurrencySourceAndTargetId(rateQueryParams, cancellationToken);

        var items = rates.Items.Select(i => i.ToExchangeRateDto()).ToList();

        return new PagedList<ExchangeRateDto>(
            items,
            rates.CurrentPage,
            rates.PageSize,
            rates.TotalCount);
    }
}