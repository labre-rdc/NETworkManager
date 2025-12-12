using System;
using System.Globalization;
using System.Windows.Data;


namespace NETworkManager.Converters;

public class EnumToIntConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType.IsEnum)
        {
            return value is null ? string.Empty :
                Enum.GetName(targetType, value);
        }
        if (value is Enum enumVal)
        {
            return Array.IndexOf(Enum.GetValues(enumVal.GetType()), enumVal);
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Convert(value, targetType, parameter, culture);
    }
}