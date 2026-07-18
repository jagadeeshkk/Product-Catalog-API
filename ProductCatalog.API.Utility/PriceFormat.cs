using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ProductCatalog.API.Utility
{
    [ExcludeFromCodeCoverage]
    public static class PriceFormat
    {
        private static readonly Regex Pattern =
            new(@"^\$(?<amount>[0-9]+(\.[0-9]+)?)(?<unit>/.+)$", RegexOptions.Compiled);

        public static (decimal Amount, string Unit) Parse(string formattedPrice)
        {
            var match = Pattern.Match(formattedPrice.Trim());
            if (!match.Success)
            {
                throw new FormatException($"Unrecognized price format: '{formattedPrice}'");
            }

            var amount = decimal.Parse(match.Groups["amount"].Value, CultureInfo.InvariantCulture);
            var unit = match.Groups["unit"].Value;
            return (amount, unit);
        }

        public static string Format(decimal amount, string unit) =>
            $"${amount.ToString("0.00", CultureInfo.InvariantCulture)}{unit}";
    }
}
