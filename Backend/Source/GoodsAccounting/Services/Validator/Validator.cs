using System.Text.RegularExpressions;

namespace GoodsAccounting.Services.Validator;

/// <summary>
/// Data validator.
/// </summary>
public class Validator : IPasswordValidator, IDtoValidator
{
    /// <inheritdoc />
    public bool Validate(string value)
    {
        const int minPasswordLength = 8;
        var regexWordUp = new Regex(@"[A-Z]");
        var regexWordLow = new Regex(@"[a-z]");
        var regexDigit = new Regex(@"\d");
        var regexNotWord = new Regex(@"\W");
        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (value.Length < minPasswordLength)
            return false;
        
        return regexWordUp.IsMatch(value) 
               && regexWordLow.IsMatch(value) 
               && regexDigit.IsMatch(value) 
               && regexNotWord.IsMatch(value);
    }
}