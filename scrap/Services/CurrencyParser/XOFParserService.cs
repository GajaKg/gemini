
using gemini.Services.Email;
using gemini.Utilities;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Scrap.Domain.Enums;
using Scrap.Domain.Interfaces;
using Scrap.Domain.Models;

namespace gemini.Services.CurrencyParser
{
    public class XOFParserService : IXofParserService
    {
        private readonly ILogger<XOFParserService> _logger;
        private readonly IEmailService _email;
        private readonly IConfiguration _configuration;
        
        public XOFParserService(ILogger<XOFParserService> logger, IEmailService email, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _email = email;
        }

        /// <summary>
        /// Extract data from html
        /// </summary>
        public async Task<List<ExchangeRateRaw>?> Parse(HtmlDocument doc, CancellationToken cancellationToken)
        {
            // <tr><td>EUR</td><td>655,957</td><td>655,957</td></tr>
            var rows = doc.DocumentNode.SelectNodes("//table/tbody/tr");
            var exchangeRateTitle = doc.DocumentNode.SelectSingleNode("//h2[normalize-space()='Exchange rates of']");

            if (rows is null && exchangeRateTitle is null)
            {
                _logger.LogCritical("Please check page for html changes!!!");
                var errorReciever = _configuration["Email:ErrorReciever"];

                if (errorReciever is null) return null;
                await _email.ComposeMessage(errorReciever, "XOF scraper critical error", "Please check page for html changes!", cancellationToken);

                return null;
            }
            
            if (rows is null)
            {
                _logger.LogWarning("⚠️  Missing currency or please check page for html changes.");
                return null;
            }

            List<ExchangeRateRaw> ratesList = [];

            foreach (var row in rows.Skip(1))
            {
                var cells = row.SelectNodes("./td");

                // Parsing currency to CurrencyCode
                if (!Enum.TryParse<CurrencyCode>(
                    cells[0].InnerText.Trim(),
                    ignoreCase: true,
                    out var currency))
                {
                    // _logger.LogWarning("Skip currency: {Currency}", cells[0].InnerText);
                    continue;
                }

                // parse only EUR and USD
                // for new currencies add here 
                if (currency != CurrencyNames.EUR
                    && currency != CurrencyNames.USD) continue;

                /**
                * check if value exists and transform from 655,957 to 655.957
                * <td>655,957</td>
                */
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