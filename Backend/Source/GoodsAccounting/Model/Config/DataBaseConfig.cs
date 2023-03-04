namespace GoodsAccounting.Model.Config;

/// <summary>
/// Configure data for data base.
/// </summary>
public class DataBaseConfig
{
    /// <summary>
    /// Get or set application main data base connection string.
    /// </summary>
    public string? ConnectionString { get; set; } = null!;

    /// <summary>
    /// Get or set maximal attempts count to data base connect.
    /// </summary>
    public int MaxRetryCount { get; set; }

    /// <summary>
    /// Get or set timeout for command execution.
    /// </summary>
    public int CommandTimeout { get; set; }

    /// <summary>
    /// Get or set value in indicating that data base provider returns detailed errors.
    /// </summary>
    public bool EnableDetailedErrors { get; set; }
}