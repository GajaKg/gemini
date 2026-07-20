
using gemini.Services;
using HtmlAgilityPack;

namespace scrap.Services.XOF
{
    public class XOFScraperService : IScraperService
    {
        // https://www.bceao.int/en/cours/cours-des-devises-contre-Franc-CFA-appliquer-aux-transferts";
        readonly string baseUrl = "https://www.bceao.int/en/cours/get_all_devise_by_date";
        private readonly HttpClient _httpClient;
        private readonly IParserService _parser;

        public XOFScraperService(HttpClient httpClient, IParserService parser)
        {
            _parser = parser;
            _httpClient = httpClient;
        }

        public async Task RunAsync()
        {
            Console.WriteLine("Scraping started....");

            var start = new DateOnly(2020, 1, 1);
            var end = DateOnly.FromDateTime(DateTime.Today);
            for (var date = start; date <= end; date = date.AddDays(1))
            {
                string urlByDay = $"{baseUrl}?dateJour={date.Year}-{date.Month}-{date.Day}";
                Console.WriteLine(urlByDay);
                string html = await _httpClient.GetStringAsync(urlByDay);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var currencyParsed = await _parser.Parse(doc, date);
            Console.WriteLine(date.ToShortDateString());
            Console.WriteLine(currencyParsed?.ToString());
            }
            Console.WriteLine("-----------------------------------------------------------");
        }

        // public async Task Scrap(string url)
        // {
        //     string html = await _httpClient.GetStringAsync(url);

        //     var doc = new HtmlDocument();
        //     doc.LoadHtml(html);

        //     var rows = doc.DocumentNode.SelectNodes("//table/tbody/tr");
        //     var header = doc.DocumentNode.SelectSingleNode("//h2");
        //     // Console.WriteLine(rows);
        //     if (rows is null) return;

        //     int delay = Random.Shared.Next(4000, 10001);
        //     await Task.Delay(delay);

        //     foreach (var row in rows.Skip(1))
        //     {
        //         var cells = row.SelectNodes("./td");

        //         string currency = cells[0].InnerText.Trim();
        //         if (currency == CurrencyNames.EuroXOF)
        //         {
        //             // string purchase = cells[1].InnerText.Trim();
        //             // string sale = cells[2].InnerText.Trim();
        //             decimal purchase = decimal.Parse(cells[1].InnerText.Trim(), CultureInfo.GetCultureInfo("fr-FR"));
        //             decimal sell = decimal.Parse(cells[2].InnerText.Trim(), CultureInfo.GetCultureInfo("fr-FR"));
        //             decimal middleCourse = (purchase + sell) / 2;
        //             Console.WriteLine(header.InnerText + ": " + middleCourse);
        //             break;
        //         }
        //     }
        //     Console.WriteLine("-----------------------------------------------------------");

        // }
    }
}