using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GoodsAccounting.Services.SecurityKey;

/// <summary>
/// Security key generator.
/// </summary>
public class SecurityKeyExtractor : ISecurityKeyExtractor
{
    /// <inheritdoc />
    public SymmetricSecurityKey Extract()
    {
        // It's a stub for develop mode.
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes("t7w!z%C*F-JaNdRfUjXn2r5u8x/A?D(G"));
    }
}