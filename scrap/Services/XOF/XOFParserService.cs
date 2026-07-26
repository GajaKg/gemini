
using System.Globalization;
using gemini.Interfaces;
using gemini.Models;
using HtmlAgilityPack;

namespace gemini.Services
{
    public class XOFParserService : IParserService
    {
        public ExchangeRate? Parse(HtmlDocument doc, DateOnly date, int currencyId)
        // public ExchangeRate? Parse(HtmlDocument doc)
        {
            var rows = doc.DocumentNode.SelectNodes("//table/tbody/tr");
            // var header = doc.DocumentNode.SelectSingleNode("//h2");
            // Console.WriteLine(rows);
            if (rows is null) {
                Console.WriteLine("HTML tag is missing, please check page html changes.");
                return null;
            };

            foreach (var row in rows.Skip(1))
            {
                var cells = row.SelectNodes("./td");
                string currency = cells[0].InnerText.Trim();

                if (currency == CurrencyNames.EUR)
                {
                    decimal purchase = decimal.Parse(cells[1].InnerText.Trim(), CultureInfo.GetCultureInfo("fr-FR"));
                    decimal sell = decimal.Parse(cells[2].InnerText.Trim(), CultureInfo.GetCultureInfo("fr-FR"));
                    decimal middleCourse = (purchase + sell) / 2;
                    return new ExchangeRate
                    {
                        CurrencyId = currencyId,
                        Sell = sell,
                        Buy = purchase,
                        Middle = middleCourse,
                        Date = date
                    };
                }
            }

            return null;
        }
    }
}