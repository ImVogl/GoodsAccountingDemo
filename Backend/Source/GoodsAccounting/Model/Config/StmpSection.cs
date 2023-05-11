namespace GoodsAccounting.Model.Config;

/// <summary>
/// Application config file section.
/// </summary>
public class StmpSection
{
    /// <summary>
    /// Get or ser email service server address.
    /// </summary>
    public string? Server { get; set; } = null!;

    /// <summary>
    /// Get or set email service server port.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Get or set service account login.
    /// </summary>
    public string? AccountLogin { get; set; } = null!;

    /// <summary>
    /// Get or set service account password.
    /// </summary>
    public string? AccountPassword { get; set; } = null!;
}