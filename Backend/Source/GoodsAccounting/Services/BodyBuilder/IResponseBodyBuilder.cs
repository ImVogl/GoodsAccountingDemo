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
    Dictionary<string, string> InvalidDtoBuild();

    /// <summary>
    /// Build body for unknown exception response.
    /// </summary>
    /// <returns>Response body.</returns>
    Dictionary<string, string> UnknownBuild();

    /// <summary>
    /// Build body for success response with token.
    /// </summary>
    /// <param name="token">JSON web token.</param>
    /// <returns>Response body.</returns>
    Dictionary<string, string> TokenBuild(string token);
}