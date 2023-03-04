namespace GoodsAccounting.Model.Config;

/// <summary>
/// JWT config section.
/// </summary>
public class JwtSection
{
    /// <summary>
    /// Get or set JWT parameters valid issuer.
    /// </summary>
    public string? ValidIssuer { get; set; } = null!;

    /// <summary>
    /// Get or set JWT parameters valid audience.
    /// </summary>
    public string? ValidAudience { get; set; } = null!;
}