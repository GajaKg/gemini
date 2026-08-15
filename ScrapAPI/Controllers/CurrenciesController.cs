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
        public async Task<ActionResult<IEnumerable<CurrencyDto>>> GetAll()
        {
            var currencies = await _currencyService.GetAllAsync();
            return Ok(currencies);
        }

        [HttpGet]
        [Route("target")]
        public async Task<ActionResult<IEnumerable<CurrencyDto>>> GetAllTarget()
        {
            var currencies = await _currencyService.GetAllTargetAsync();
            return Ok(currencies);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CurrencyDto>> GetById([FromRoute] int id)
        {
            var foundCurrency = await _currencyService.GetByIdAsync(id);

            if (foundCurrency is null) return NotFound();

            return Ok(foundCurrency);
        }

        [HttpGet]
        [Route("target/{id}")]
        public async Task<ActionResult<CurrencyDto>> GetByIdTarget([FromRoute] int id)
        {
            var foundCurrency = await _currencyService.GetByIdTargetAsync(id);

            if (foundCurrency is null) return NotFound();

            return Ok(foundCurrency);
        }

        [HttpGet]
        [Route("list")]
        public async Task<ActionResult<CurrencyDto>> GetByIdTarget([FromQuery] ExchangeRateQueryParams queryParams)
        {
            var foundCurrency = await _currencyService.GetByIdAndRateCurrencyIdAsync(queryParams.Id, queryParams.TargetCyrrencyId);

            if (foundCurrency is null) return NotFound();

            return Ok(foundCurrency);
        }
    }
}