using Microsoft.AspNetCore.Mvc;
using ScrapAPI.Dto;
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
        public async Task<ActionResult<IEnumerable<CurrencyWithoutRatesDto>>> GetAll(CancellationToken cancellationToken)
        {
            var currencies = await _currencyService.GetAllAsync(cancellationToken);
            return Ok(currencies);
        }

    }
}