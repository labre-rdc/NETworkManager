using System;
using System.Globalization;
using System.Windows.Data;
using NETworkManager.Models.Network;
using NETworkManager.Localization.Resources;


namespace NETworkManager.Converters;

public class BoolArrayToFwRuleCategoriesConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        const int expectedLength = 3;
        var fallback = GetTranslation(Enum.GetName(NetworkCategory.NotConfigured), false);
        if (value is not bool[] { Length: expectedLength } boolArray)
            return fallback;
        var result = string.Empty;
        var numSelected = boolArray.CountAny(true);
        switch (numSelected)
        {
            case 0:
                return fallback;
            case < 2:
                return GetTranslation(Enum.GetName(typeof(NetworkCategory),
                    Array.FindIndex(boolArray, b => true)), false);
        }

        for (var i = 0; i < expectedLength; i++)
        {
            if (boolArray[i])
                result += $", {GetTranslation(Enum.GetName(typeof(NetworkCategory), i + 1), true)}";
        }
        return result[2..];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private string GetTranslation(string key, bool trimmed)
    {
        return Strings.ResourceManager.GetString(trimmed ? $"{key}_Short3" : key);
    }
}