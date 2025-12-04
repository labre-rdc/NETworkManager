using System;
using System.Windows.Data;

namespace NETworkManager.Converters;

/// <summary>
/// Convert an enum type to a boolean and vice versa.
/// </summary>
/// <remarks>
/// Source: https://stackoverflow.com/a/2908885
/// Original author: Scott
/// https://stackoverflow.com/users/173289/scott
/// Licensed under CC-BY SA 4.0
/// https://creativecommons.org/licenses/by-sa/4.0/
/// 
/// No modifications were made to the source except for renaming the class.
/// </remarks>
public sealed class BooleanToEnumConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return value?.Equals(parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return value?.Equals(true) == true ? parameter : Binding.DoNothing;
    }
}