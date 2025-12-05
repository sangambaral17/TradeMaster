using System.Globalization;
using System.Windows.Data;

namespace TradeMaster.Desktop.Converters;

/// <summary>
/// Converts column name and sort state to header text with sort indicator (▲/▼).
/// </summary>
public class ColumnHeaderConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is not string columnName)
            return parameter?.ToString() ?? "";

        // Get friendly column names
        var displayName = columnName switch
        {
            "Name" => "Product Name",
            "Price" => "Price",
            "StockQuantity" => "Stock Qty",
            _ => columnName
        };

        // value is the current SortColumn from ViewModel
        if (value is string currentSortColumn && currentSortColumn == columnName)
        {
            // This column is being sorted - add indicator
            // Note: We can't access SortAscending here, so we'll use a simpler approach
            return $"{displayName} ▼";
        }

        return displayName;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Multi-value converter that shows column header with sort indicator based on both column and direction.
/// </summary>
public class ColumnHeaderWithSortConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is not string columnName)
            return parameter?.ToString() ?? "";

        // Get friendly column names
        var displayName = columnName switch
        {
            "Name" => "Product Name",
            "Price" => "Price",
            "StockQuantity" => "Stock Qty",
            _ => columnName
        };

        // values[0] is SortColumn, values[1] is SortAscending
        if (values.Length >= 2 && 
            values[0] is string currentSortColumn && 
            values[1] is bool sortAscending &&
            currentSortColumn == columnName)
        {
            var indicator = sortAscending ? "▲" : "▼";
            return $"{displayName} {indicator}";
        }

        return displayName;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
