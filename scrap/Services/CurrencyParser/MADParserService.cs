using System.Text.RegularExpressions;
using gemini.Utilities;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Scrap.Domain.Enums;
using Scrap.Domain.Interfaces;
using Scrap.Domain.Models;

namespace gemini.Services.CurrencyParser;

public class MADParserService : IMadParserService
{
    private readonly ILogger<MADParserService> _logger;

    public MADParserService(ILogger<MADParserService> logger)
    {
        _logger = logger;

    }

    // <table class="dynamic_contents_ref_19">
    //   <thead>
    //     <tr>
    //       <th>
    //         <div class="block-filter">
    //           <span class="txt-filter">Currencies</span>
    //         </div>
    //       </th>
    //       <th>
    //         <div class="block-filter">
    //           <span class="txt-filter">Purchase from customers</span>
    //         </div>
    //       </th>
    //       <th>
    //         <div class="block-filter">
    //           <span class="txt-filter">Sale to customers</span>
    //         </div>
    //       </th>
    //     </tr>
    //   </thead>
    //   <tbody>
    //     <tr>
    //       <td><span class="object_name">1 EURO</span><br /></td>
    //       <td>
    //         <span class="number">10.3419&nbsp;<span class="symbol"></span></span>
    //       </td>

    //       <td>
    //         <span class="number">11.4305&nbsp;<span class="symbol"></span></span>
    //       </td>
    //     </tr>
    //     <tr>
    //       <td><span class="object_name">1 US DOLLAR</span><br /></td>
    //       <td>
    //         <span class="number">9.26690&nbsp;<span class="symbol"></span></span>
    //       </td>

    //       <td>
    //         <span class="number">10.2423&nbsp;<span class="symbol"></span></span>
    //       </td>
    //     </tr>
    //   </tbody>
    // </table>

    /// <summary>
    /// Extract data from html
    /// </summary>
    public List<ExchangeRateRaw>? Parse(HtmlDocument doc)
    {
        // <tr>
        //     <td><span class="object_name">1 EURO</span><br /></td>
        //     <td><span class="number">10.3419&nbsp;<span class="symbol"></span></span></td>
        //     <td><span class="number">11.4305&nbsp;<span class="symbol"></span></span></td>
        // </tr>
        var rows = doc.DocumentNode.SelectNodes("//tbody/tr");

        if (rows is null)
        {
            _logger.LogWarning("⚠️  Missing currency or please check page for html changes.");
            return null;
        }

        List<ExchangeRateRaw> ratesList = [];

        foreach (var row in rows)
        {
            // get currency from table
            string? currency = row.SelectSingleNode("./td[1]")?.InnerText.Trim();
            CurrencyCode? currencyCode = ParseCurrencyCode(currency ?? "");

            // parse only EUR and USD
            // for new currencies add here 
            if (currencyCode != CurrencyNames.EUR
                && currencyCode != CurrencyNames.USD) continue;

            /**
            * check if value exists and transform to eg. 555.33
            * <td>
            *   <span class="number">10.3419&nbsp;<span class="symbol"></span></span>
            * </td>
            */
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

    /// <summary>
    /// Table column titles contains data for  
    /// "Euro" and "US  Dollar"
    /// </summary>
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