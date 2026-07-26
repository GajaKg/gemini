

using System.Globalization;
using gemini.Models;
using HtmlAgilityPack;

namespace gemini.Services.MAD
{
    public class MADParserService : IParserService
    {
        public ExchangeRate? Parse(HtmlDocument doc, DateOnly date, int currencyId)
        {

            var row = doc.DocumentNode.SelectSingleNode("//tbody/tr[1]");
            // IWebElement row = doc.FindElement(By.CssSelector("tbody > tr:first-child"));

            if (row is null) {
                Console.WriteLine("HTML tag is missing, please check page html changes.");
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
                    Console.WriteLine($"Invalid purchase value: {purchaseValue}");
                    return null;
                }

                if (!decimal.TryParse(sellValue, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal sell))
                {
                    Console.WriteLine($"Invalid sell value: {sellValue}");
                    return null;
                }

                decimal middleCourse = (purchase + sell) / 2;
                Console.WriteLine("Srednji kurs na dan " + date.ToString() + ": " + middleCourse);

                return new ExchangeRate
                {
                    CurrencyId = currencyId,
                    Sell = sell,
                    Buy = purchase,
                    Middle = middleCourse,
                    Date = date
                };
            }
            return null;
        }
    }
}