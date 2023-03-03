using Microsoft.IdentityModel.Tokens;

namespace GoodsAccounting.Services.SecurityKey;

/// <summary>
/// Interface for key generator.
/// </summary>
public interface ISecurityKeyExtractor
{
    /// <summary>
    /// Extract cypher key.
    /// </summary>
    /// <returns>Key.</returns>
    SymmetricSecurityKey Extract();
}