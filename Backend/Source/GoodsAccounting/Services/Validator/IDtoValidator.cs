using GoodsAccounting.Model.DTO;
using JetBrains.Annotations;

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
    bool Validate([NotNull]SignInDto dto);
}