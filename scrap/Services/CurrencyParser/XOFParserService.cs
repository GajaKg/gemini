
using gemini.Interfaces;
using gemini.Models;
using gemini.Utilities;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace gemini.Services.CurrencyParser
{
    public class XOFParserService : IParserService
    {
        private readonly ILogger<XOFParserService> _logger;
        public XOFParserService(ILogger<XOFParserService> logger)
        {
            _logger = logger;
        }

        public List<ExchangeRateRaw>? Parse(HtmlDocument doc)
        {
            var rows = doc.DocumentNode.SelectNodes("//table/tbody/tr");

            if (rows is null)
            {
                _logger.LogWarning("⚠️  Missing currency or please check page for html changes.");
                return null;
            }

            List<ExchangeRateRaw> ratesList = [];

            foreach (var row in rows.Skip(1))
            {
                var cells = row.SelectNodes("./td");
                // CurrencyCode currency = cells[0].InnerText.Trim();
                if (!Enum.TryParse<CurrencyCode>(
                    cells[0].InnerText.Trim(),
                    ignoreCase: true,
                    out var currency))
                {
                    // _logger.LogWarning("Skip currency: {Currency}", cells[0].InnerText);
                    continue;
                }


                if (currency != CurrencyNames.EUR
                    && currency != CurrencyNames.USD) continue;

                decimal? purchase = CurrencyNormalize.ExtractNormalizeValueCultureFr(cells[1].InnerText);
                if (purchase is null)
                {
                    _logger.LogWarning("⚠️  Invalid purchase value: {Purchase}", purchase);
                    continue;
                }

                decimal? sell = CurrencyNormalize.ExtractNormalizeValueCultureFr(cells[2].InnerText);
                if (sell is null)
                {
                    _logger.LogWarning("⚠️  Invalid sell value: {Sell}", sell);
                    continue;
                }

                decimal middleCourse = (purchase.Value + sell.Value) / 2;

                ratesList.Add(
                    new ExchangeRateRaw
                    {
                        Sell = sell.Value,
                        Buy = purchase.Value,
                        Middle = middleCourse,
                        TargetCurrency = currency
                    }
                );
            }

            return ratesList;
        }
    }
}