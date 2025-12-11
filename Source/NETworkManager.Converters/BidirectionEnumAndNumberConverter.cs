using System;
using System.Globalization;
using System.Windows.Data;

namespace NETworkManager.Converters;

/// <summary>
/// Converts Enum to a number and vice versa
/// </summary>
/// <remarks>
/// Source: https://stackoverflow.com/a/33611155
/// License: CC-BY-SA 3.0
/// https://creativecommons.org/licenses/by-sa/4.0/legalcode.en
/// Author: https://stackoverflow.com/users/24874/drew-noakes
/// No modifications were made to the code.
/// </remarks>
public class BidirectionEnumAndNumberConverter
{
    public sealed class BidirectionalEnumAndNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (targetType.IsEnum)
            {
                // convert int to enum
                return Enum.ToObject(targetType, value);
            }

            if (value.GetType().IsEnum)
            {
                // convert enum to int
                return System.Convert.ChangeType(
                    value,
                    Enum.GetUnderlyingType(value.GetType()));
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // perform the same conversion in both directions
            return Convert(value, targetType, parameter, culture);
        }
    }    
}