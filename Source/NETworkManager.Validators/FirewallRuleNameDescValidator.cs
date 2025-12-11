using System.Globalization;
using System.Windows.Controls;
using NETworkManager.Localization.Resources;

namespace NETworkManager.Validators;

public class FirewallRuleNameDescValidator : ValidationRule
{
    /// <summary>
    /// Two conditions:
    /// - '|' must not be contained
    /// - String must not be longer than 9999 characters
    /// </summary>
    /// <param name="value">Value to be checked</param>
    /// <param name="cultureInfo">Current language info</param>
    /// <returns></returns>
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is not string strVal)
            return new ValidationResult(false, "Internal error");
        if (strVal.Length > 9999)
            return new ValidationResult(false,
                "String length was not limited to 9999 characters. This is a bug.");
        if (strVal.Contains('|'))
            return new ValidationResult(false, Strings.PipeNotAllowed);
        return ValidationResult.ValidResult;
    }

}