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

    /// <summary>
    /// Validation of <see cref="GoodsRevisionDto"/>.
    /// </summary>
    /// <param name="dto"><see cref="GoodsRevisionDto"/>.</param>
    /// <returns>Value in indicating that DTO is valid.</returns>
    bool Validate([NotNull] GoodsRevisionDto dto);

    /// <summary>
    /// Validation of <see cref="GoodsSuppliesDto"/>.
    /// </summary>
    /// <param name="dto"><see cref="GoodsSuppliesDto"/>.</param>
    /// <returns>Value in indicating that DTO is valid.</returns>
    bool Validate([NotNull] GoodsSuppliesDto dto);

    /// <summary>
    /// Validation of <see cref="EditGoodsListDto"/>.
    /// </summary>
    /// <param name="dto"><see cref="EditGoodsListDto"/>.</param>
    /// <returns>Value in indicating that DTO is valid.</returns>
    bool Validate([NotNull] EditGoodsListDto dto);
}