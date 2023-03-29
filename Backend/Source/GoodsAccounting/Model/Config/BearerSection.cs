namespace GoodsAccounting.Model.Config;

/// <summary>
/// JWT config section.
/// </summary>
public class BearerSection
{
    /// <summary>
    /// Get or set JWT parameters valid issuer.
    /// </summary>
    public string? ValidIssuer { get; set; } = null!;

    /// <summary>
    /// Get or set JWT parameters valid audience.
    /// </summary>
    public string? ValidAudience { get; set; } = null!;

    /// <summary>
    /// Get or set origin.
    /// </summary>
    public string? Origin { get; set; } = null!;

    /// <summary>
    /// Get or set path to .pem file with secret key
    /// </summary>
    public string? PathToPem { get; set; } = null!;
}