using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EMS.App.Converters;

public class InverseVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool val && val
            ? Visibility.Collapsed : Visibility.Visible; 
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
