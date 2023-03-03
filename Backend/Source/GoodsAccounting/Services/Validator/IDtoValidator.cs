using GoodsAccounting.Model.DTO;

namespace GoodsAccounting.Services.Validator;

/// <summary>
/// Interface for DTO validators.
/// </summary>
public interface IDtoValidator
{
    /// <summary>
    /// Validation of <see cref="SignInDto"/> DTO.
    /// </summary>
    /// <param name="dto"><see cref="SignInDto"/></param>
    /// <returns>Value in indicating that DTO is valid.</returns>
    bool Validate(SignInDto dto);
}