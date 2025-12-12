using System;
using System.Collections;
using System.Globalization;
using System.Management.Automation;
using System.Resources;
using System.Windows.Data;
using NETworkManager.Localization;
using NETworkManager.Localization.Resources;

namespace NETworkManager.Converters;

public sealed class EnumToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Enum enumValue)
        {
            object fallback = Enum.ToObject(targetType, 0);
            if (value is not string strVal)
                return fallback;
            ResourceSet resourceSet = Strings.ResourceManager.GetResourceSet(LocalizationManager.GetInstance().Culture,
                false, true);
            string foundKey = null;
            if (resourceSet is null)
                return fallback;
            foreach (DictionaryEntry item in resourceSet)
            {
                if (item.Value as string == strVal || item.Key as string == strVal)
                {
                    foundKey = item.Key as string;
                    break;
                }
            }

            if (foundKey is null || !Enum.TryParse(targetType, foundKey, out var result))
                return fallback;
            return result;
        }
        
        var enumString = Enum.GetName((enumValue.GetType()), value);
        if (enumString is null)
            return string.Empty;
        return Strings.ResourceManager.GetString(enumString) ?? enumString;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Convert(value, targetType, parameter, culture);
    }
}