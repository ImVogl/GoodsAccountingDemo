namespace GoodsAccounting.Services.Validator;

/// <summary>
/// Interface for password validator.
/// </summary>
public interface IPasswordValidator
{
    /// <summary>
    /// Password validator
    /// </summary>
    /// <param name="value">Password.</param>
    /// <returns>Value is indicating that password is valid.</returns>
    bool Validate(string value);
}