using System.Text.RegularExpressions;
using gemini.Interfaces;
using gemini.Models;
using gemini.Utilities;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace gemini.Services.CurrencyParser
{
    public class MADParserService : IParserService
    {
        private readonly ILogger<MADParserService> _logger;

        public MADParserService(ILogger<MADParserService> logger)
        {
            _logger = logger;

        }
        public List<ExchangeRateRaw>? Parse(HtmlDocument doc)
        {
            var rows = doc.DocumentNode.SelectNodes("//tbody/tr");

            if (rows is null)
            {
                _logger.LogWarning("⚠️  Missing currency or please check page for html changes.");
                return null;
            }

            List<ExchangeRateRaw> ratesList = [];

            foreach (var row in rows)
            {
                string? currency = row.SelectSingleNode("./td[1]")?.InnerText.Trim();
                CurrencyCode? currencyCode = ParseCurrencyCode(currency ?? "");

                if (currencyCode != CurrencyNames.EUR
                    && currencyCode != CurrencyNames.USD) continue;

                decimal? purchase = CurrencyNormalize.ExtractNormalizeValueCultureUs(row.SelectSingleNode("./td[2]//span[@class='number']").InnerText);
                if (purchase is null)
                {
                    _logger.LogWarning("⚠️  Invalid purchase value: {Purchase}", purchase);
                    continue;
                }

                decimal? sell = CurrencyNormalize.ExtractNormalizeValueCultureUs(row.SelectSingleNode("./td[2]//span[@class='number']").InnerText);
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
                        TargetCurrency = currencyCode.Value
                    }
                );
            }

            return ratesList;
        }

        private static CurrencyCode? ParseCurrencyCode(string value)
        {
            value = value.ToUpperInvariant();

            if (Regex.IsMatch(value, @"\bEURO\b"))
                return CurrencyCode.EUR;

            if (Regex.IsMatch(value, @"\bUS\s+DOLLAR\b"))
                return CurrencyCode.USD;

            return null;
        }

    }
}