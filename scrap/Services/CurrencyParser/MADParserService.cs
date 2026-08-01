

using System.Globalization;
using gemini.Models;
using HtmlAgilityPack;

namespace gemini.Services.CurrencyParser
{
    public class MADParserService : IParserService
    {
        public ExchangeRateRaw? Parse(HtmlDocument doc)
        {

            var row = doc.DocumentNode.SelectSingleNode("//tbody/tr[1]");
            // IWebElement row = doc.FindElement(By.CssSelector("tbody > tr:first-child"));

            if (row is null) 
            {
                Console.WriteLine("⚠️  Missing currency or please check page for html changes.");
                return null;
            };

            string? currency = row.SelectSingleNode("./td[1]")?.InnerText.Trim();
            string? purchaseValue = HtmlEntity.DeEntitize(
                row.SelectSingleNode("./td[2]//span[@class='number']")?.InnerText ?? ""
            ).Trim();

            string? sellValue = HtmlEntity.DeEntitize(
                row.SelectSingleNode("./td[3]//span[@class='number']")?.InnerText ?? ""
            ).Trim();

            if (
                currency == "1 EURO" &&
                purchaseValue != null &&
                sellValue != null
            )
            {
                if (!decimal.TryParse(purchaseValue, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal purchase))
                {
                    Console.WriteLine($"⚠️  Invalid purchase value: {purchaseValue}");
                    return null;
                }

                if (!decimal.TryParse(sellValue, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal sell))
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