using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TradeMaster.Desktop.Converters;

/// <summary>
/// Converts an empty or null string to Visibility.Visible (to show placeholder icon).
/// </summary>
public class EmptyStringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str && !string.IsNullOrWhiteSpace(str))
        {
            return Visibility.Collapsed;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a page number to boolean (true if page > 1, for Previous button).
/// </summary>
public class PageGreaterThan1Converter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int page)
        {
            return page > 1;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converter that requires both CurrentPage and TotalPages to determine if Next button should be enabled.
/// This is a simplified version that just checks if value is not equal to a bound TotalPages.
/// </summary>
public class PageLessThanTotalConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 && values[0] is int currentPage && values[1] is int totalPages)
        {
            return currentPage < totalPages;
        }
        return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
