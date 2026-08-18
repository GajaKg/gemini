using Microsoft.AspNetCore.Mvc;
using ScrapAPI.Dto;
using ScrapAPI.Helpers;
using ScrapAPI.Services;

namespace ScrapAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExchangeRatesController : ControllerBase
{
    private readonly IExchangeRateService _exchangeRateService;

    public ExchangeRatesController(IExchangeRateService exchangeRateService)
    {
        _exchangeRateService = exchangeRateService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<ExchangeRateDto>>> GetRates([FromQuery] ExchangeRateQueryParams rateQueryParams, CancellationToken cancellationToken)
    {
        var rates = await _exchangeRateService.GetRatesByCurrencySourceAndTargetId(rateQueryParams.Id, rateQueryParams.TargetCurrencyId, rateQueryParams.Date, rateQueryParams, cancellationToken);

        return Ok(rates);
    }
}