using System;
using System.Globalization;
using System.Windows.Data;
using NETworkManager.Localization;
using NETworkManager.Localization.Resources;

namespace NETworkManager.Converters;

public sealed class EnumToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Enum enumValue)
            return string.Empty;

        var enumString = Enum.GetName((enumValue.GetType()), value);
        return enumString is not null ? Strings.ResourceManager.GetString(enumString) : string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!targetType.IsEnum)
            return string.Empty;
        object fallback = Enum.ToObject(targetType, 0);
        if (value is not string)
            return fallback;
        string valStr = (string)value;
        var resourceSet = Strings.ResourceManager.GetResourceSet(LocalizationManager.GetInstance().Culture,
            false, true);
        string foundKey = null;
        // ReSharper disable once GenericEnumeratorNotDisposed
        var enumerator = resourceSet?.GetEnumerator();
        if (enumerator is null)
            return fallback;
        while (enumerator.MoveNext())
        {
            if (enumerator.Value as string == valStr)
            {
                foundKey = enumerator.Key as string;
                break;
            }
        }
        resourceSet?.Dispose();

        return foundKey is not null
            ? Enum.TryParse(targetType, foundKey, out var result)
            : fallback;
    }
}