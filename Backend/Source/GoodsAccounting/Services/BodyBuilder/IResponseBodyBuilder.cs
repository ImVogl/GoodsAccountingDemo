using JetBrains.Annotations;

namespace GoodsAccounting.Services.BodyBuilder;

/// <summary>
/// Interface for request body builder service.
/// </summary>
public interface IResponseBodyBuilder
{
    /// <summary>
    /// Build body for invalid DTO response.
    /// </summary>
    /// <returns>Response body.</returns>
    [NotNull]
    [ItemNotNull]
    Dictionary<string, string> InvalidDtoBuild();

    /// <summary>
    /// Build body for unknown exception response.
    /// </summary>
    /// <returns>Response body.</returns>
    [NotNull]
    [ItemNotNull]
    Dictionary<string, string> UnknownBuild();
    
    /// <summary>
    /// Build body for entity exists already in storage response.
    /// </summary>
    /// <returns>Response body.</returns>
    [NotNull]
    [ItemNotNull]
    Dictionary<string, string> EntityExistsBuild();

    /// <summary>
    /// Build body for entity doesn't exist in storage response.
    /// </summary>
    /// <returns>Response body.</returns>
    [NotNull]
    [ItemNotNull]
    Dictionary<string, string> EntityNotFoundBuild();
}