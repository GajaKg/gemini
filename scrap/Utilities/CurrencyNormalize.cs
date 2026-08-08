using System.Globalization;
using HtmlAgilityPack;

namespace gemini.Utilities
{
    public class CurrencyNormalize
    {
        public static decimal? ExtractNormalizeValueCultureFr(string value = "")
        {
            string normalizedValue = DeEntitize(value);

            if (!decimal.TryParse(normalizedValue, NumberStyles.Number, CultureInfo.GetCultureInfo("fr-FR"), out decimal parsedValue))
            {
                return null;
            }

            return parsedValue;
        }

        public static decimal? ExtractNormalizeValueCultureUs(string value = "")
        {
            string normalizedValue = DeEntitize(value);

            if (!decimal.TryParse(normalizedValue, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsedValue))
            {
                return null;
            }

            return parsedValue;
        }

        private static string DeEntitize(string value)
        {
            return HtmlEntity.DeEntitize(value.Trim()).Trim();
        }
    }
}