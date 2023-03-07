using Newtonsoft.Json;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// DTO with supply information for item.
/// </summary>
public class GoodsItemSupplyDto
{
    /// <summary>
    /// Get or set item identifier.
    /// </summary>
    [JsonRequired]
    [JsonProperty("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Get or set receipted items count.
    /// </summary>
    [JsonRequired]
    [JsonProperty("receipt")]
    public int Receipt { get; set; }

    /// <summary>
    /// Get or set wholescale price of item.
    /// </summary>
    [JsonRequired]
    [JsonProperty("price")]
    public float WholeScalePrice { get; set; }
}