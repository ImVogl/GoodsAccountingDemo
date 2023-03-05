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
    /// Build body for success response with token.
    /// </summary>
    /// <param name="token">JSON web token.</param>
    /// <returns>Response body.</returns>
    [NotNull]
    [ItemNotNull]
    Dictionary<string, string> TokenBuild(string token);

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

    /// <summary>
    /// Build body for success response new user info.
    /// </summary>
    /// <param name="login">New user login.</param>
    /// <param name="password">New user password.</param>
    /// <param name="token">Token.</param>
    /// <returns></returns>
    [NotNull]
    [ItemNotNull]
    Dictionary<string, string> TokenNewUserBuild(string login, string password, string token);
}