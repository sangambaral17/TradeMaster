using System;
using System.Globalization;
using System.Windows.Data;
using TradeMaster.Desktop.Helpers;

namespace TradeMaster.Desktop.Converters
{
    /// <summary>
    /// Converts decimal values to NPR currency format for display in UI
    /// </summary>
    public class CurrencyToNPRConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                return CurrencyHelper.FormatNPR(decimalValue);
            }

            if (value is double doubleValue)
            {
                return CurrencyHelper.FormatNPR((decimal)doubleValue);
            }

            if (value is int intValue)
            {
                return CurrencyHelper.FormatNPR(intValue);
            }

            return "Rs. 0.00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                // Remove currency symbol and parse
                string cleanValue = stringValue.Replace("Rs.", "").Replace(",", "").Trim();
                if (decimal.TryParse(cleanValue, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal result))
                {
                    return result;
                }
            }

            return 0m;
        }
    }
}
