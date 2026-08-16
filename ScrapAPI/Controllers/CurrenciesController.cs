using Microsoft.AspNetCore.Mvc;
using ScrapAPI.Dto;
using ScrapAPI.Helpers;
using ScrapAPI.Services;

namespace ScrapAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrenciesController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CurrencyDto>>> GetAll(CancellationToken cancellationToken)
        {
            var currencies = await _currencyService.GetAllAsync(cancellationToken);
            return Ok(currencies);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CurrencyDto>> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var foundCurrency = await _currencyService.GetByIdAsync(id, cancellationToken);

            if (foundCurrency is null) return NotFound();

            return Ok(foundCurrency);
        }

        [HttpGet]
        [Route("list")]
        public async Task<ActionResult<CurrencyDto>> GetByIdTarget([FromQuery] ExchangeRateQueryParams queryParams, CancellationToken cancellationToken)
        {
            var foundCurrency = await _currencyService.GetByIdAndRateCurrencyIdAsync(queryParams.Id, queryParams.TargetCyrrencyId, cancellationToken);

            if (foundCurrency is null) return NotFound();

            return Ok(foundCurrency);
        }
    }
}