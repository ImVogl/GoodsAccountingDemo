using System.Security.Claims;

namespace GoodsAccounting.Services;

/// <summary>
/// Common utils.
/// </summary>
public static class Utils
{
    /// <summary>
    /// Get user identifier from JWT.
    /// </summary>
    /// <param name="context"><see cref="HttpContext"/>.</param>
    /// <returns>User identifier or null.</returns>
    public static int? ExtractUserIdentifierFromToken(HttpContext context)
    {

        if (context.User.Identity is not ClaimsIdentity identity)
            return null;

        var claim = identity.FindFirst("Id");
        if (claim == null)
            return null;

        return int.TryParse(claim.Value, out var id) ? id : null;
    }
}