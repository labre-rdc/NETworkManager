using System.Globalization;
using System.Windows.Controls;
using NETworkManager.Localization.Resources;

namespace NETworkManager.Validators;

/// <summary>
/// Provides validation logic for user-defined names or descriptions used in firewall rules.
/// Ensures the value meets specific criteria for validity:
/// - The character '|' is not allowed.
/// - The string length does not exceed 9999 characters.
/// </summary>
public class FirewallRuleNameDescValidator : ValidationRule
{
    /// <summary>
    /// Validates a string based on the following two conditions:
    /// - The string must not contain the '|' character.
    /// - The string must not exceed a length of 9999 characters.
    /// </summary>
    /// <param name="value">The value to be validated.</param>
    /// <param name="cultureInfo">The culture information used during validation.</param>
    /// <returns>A ValidationResult indicating whether the string is valid or not.</returns>
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (string.IsNullOrEmpty(value?.ToString()))
            return ValidationResult.ValidResult;
        var strVal = (string)value;
        if (strVal.Length > 9999)
            return new ValidationResult(false,
                "String length was not limited to 9999 characters. This is a bug.");
        if (strVal.Contains('|'))
            return new ValidationResult(false, Strings.PipeNotAllowed);
        return ValidationResult.ValidResult;
    }

}