namespace GoodsAccounting.Model;

/// <summary>
/// Model with information about storage state changing.
/// </summary>
public class GoodsItemStateChanging
{
    /// <summary>
    /// Get or set category of item.
    /// </summary>
    public string Category { get; set; } = null!;

    /// <summary>
    /// Get or set writted-off items count.
    /// </summary>
    public int WriteOff { get; set; }

    /// <summary>
    /// Get or set receipted items count.
    /// </summary>
    public int Receipt { get; set; }

    /// <summary>
    /// Get or set wholescale price of item.
    /// </summary>
    public float WholeScalePrice { get; set; }

    /// <summary>
    /// Get or set retail price of item.
    /// </summary>
    public float RetailPrice { get; set; }

    /// <summary>
    /// Get or set count of items in storage.
    /// </summary>
    public int Storage { get; set; }

}