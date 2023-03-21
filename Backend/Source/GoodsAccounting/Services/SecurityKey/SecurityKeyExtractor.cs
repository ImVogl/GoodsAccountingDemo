using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace GoodsAccounting.Services.SecurityKey;

/// <summary>
/// Security key generator.
/// </summary>
public class SecurityKeyExtractor : ISecurityKeyExtractor
{
    /// <summary>
    /// Path to file with secret key.
    /// </summary>
    private readonly string _pathToKey;

    /// <summary>
    /// Creating new instance of <see cref="SecurityKeyExtractor"/>.
    /// </summary>
    /// <param name="pathToKey">Path to file with secret key.</param>
    public SecurityKeyExtractor(string pathToKey)
    {
        _pathToKey = pathToKey;
    }

    /// <inheritdoc />
    public SymmetricSecurityKey Extract()
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(File.ReadAllText(_pathToKey).ToCharArray());
        return new SymmetricSecurityKey(rsa.ExportRSAPrivateKey());
    }
}