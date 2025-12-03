using System.Globalization;

namespace TradeMaster.Desktop.Helpers
{
    /// <summary>
    /// Helper class for formatting currency in Nepali Rupees (NPR)
    /// </summary>
    public static class CurrencyHelper
    {
        private static readonly NumberFormatInfo NprFormat;

        static CurrencyHelper()
        {
            // Create custom number format for NPR (Nepali Rupees)
            NprFormat = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            NprFormat.CurrencySymbol = "Rs. ";
            NprFormat.CurrencyDecimalDigits = 2;
            NprFormat.CurrencyDecimalSeparator = ".";
            NprFormat.CurrencyGroupSeparator = ",";
            NprFormat.CurrencyGroupSizes = new[] { 3 };
            NprFormat.CurrencyPositivePattern = 0; // $n
            NprFormat.CurrencyNegativePattern = 0; // ($n)
        }

        /// <summary>
        /// Formats a decimal value as NPR currency
        /// </summary>
        /// <param name="amount">The amount to format</param>
        /// <returns>Formatted currency string (e.g., "Rs. 1,234.56")</returns>
        public static string FormatNPR(decimal amount)
        {
            return amount.ToString("C", NprFormat);
        }

        /// <summary>
        /// Formats a decimal value as NPR currency without decimals
        /// </summary>
        /// <param name="amount">The amount to format</param>
        /// <returns>Formatted currency string (e.g., "Rs. 1,235")</returns>
        public static string FormatNPRRounded(decimal amount)
        {
            var format = (NumberFormatInfo)NprFormat.Clone();
            format.CurrencyDecimalDigits = 0;
            return amount.ToString("C", format);
        }
    }
}
