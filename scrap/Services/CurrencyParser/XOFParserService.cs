
using System.Globalization;
using gemini.Interfaces;
using gemini.Models;
using HtmlAgilityPack;

namespace gemini.Services.CurrencyParser
{
    public class XOFParserService : IParserService
    {
        public ExchangeRateRaw? Parse(HtmlDocument doc)
        {
            var rows = doc.DocumentNode.SelectNodes("//table/tbody/tr");
       
            if (rows is null)
            {
                Console.WriteLine("⚠️  Missing currency or please check page for html changes.");
                return null;
            }

            foreach (var row in rows.Skip(1))
            {
                var cells = row.SelectNodes("./td");
                string currency = cells[0].InnerText.Trim();

                if (currency != CurrencyNames.EUR) return null;

                string? purchaseValue = HtmlEntity.DeEntitize(
                    cells[1].InnerText.Trim() ?? ""
                ).Trim();

                string? sellValue = HtmlEntity.DeEntitize(
                    cells[2].InnerText.Trim() ?? ""
                ).Trim();

                if (!decimal.TryParse(purchaseValue, NumberStyles.Number, CultureInfo.GetCultureInfo("fr-FR"), out decimal purchase))
                {
                    Console.WriteLine($"⚠️  Invalid purchase value: {purchaseValue}");
                    return null;
                }

                if (!decimal.TryParse(sellValue, NumberStyles.Number, CultureInfo.GetCultureInfo("fr-FR"), out decimal sell))
                {
                    Console.WriteLine($"⚠️  Invalid sell value: {sellValue}");
                    return null;
                }

                decimal middleCourse = (purchase + sell) / 2;
                return new ExchangeRateRaw
                {
                    Sell = sell,
                    Buy = purchase,
                    Middle = middleCourse,
                };
            }
            return null;
        }
    }
}