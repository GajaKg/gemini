
using System.Globalization;
using gemini.Models;
using HtmlAgilityPack;

namespace gemini.Services
{
    public class XOFParserService : IParserService
    {
        public async Task<ExchangeRate?> Parse(HtmlDocument doc, DateOnly date)
        {
            var rows = doc.DocumentNode.SelectNodes("//table/tbody/tr");
            // var header = doc.DocumentNode.SelectSingleNode("//h2");
            // Console.WriteLine(rows);
            if (rows is null) return null;

            int delay = Random.Shared.Next(4000, 10001);
            await Task.Delay(delay);

            foreach (var row in rows.Skip(1))
            {
                var cells = row.SelectNodes("./td");
                string currency = cells[0].InnerText.Trim();

                if (currency == "EUR")
                {
                    decimal purchase = decimal.Parse(cells[1].InnerText.Trim(), CultureInfo.GetCultureInfo("fr-FR"));
                    decimal sell = decimal.Parse(cells[2].InnerText.Trim(), CultureInfo.GetCultureInfo("fr-FR"));
                    decimal middleCourse = (purchase + sell) / 2;
                    return new ExchangeRate
                    {
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