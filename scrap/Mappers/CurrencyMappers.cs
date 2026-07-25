using gemini.Models;
using scrap.Dtos;

namespace gemini.Mappers
{
    public static class CurrencyMappers
    {
        public static CurrencyRequest ToRequestCurrency(this Currency currency)
        {
            return new CurrencyRequest
            {
                Code = currency.Code,
                Name = currency.Name,
            };
        }
    }
}