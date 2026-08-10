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
    }
}