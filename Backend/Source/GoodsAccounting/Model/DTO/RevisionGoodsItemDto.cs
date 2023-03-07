using Newtonsoft.Json;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// DTO for updatable goods item.
/// </summary>
public class RevisionGoodsItemDto
{
    /// <summary>
    /// Get or set item identifier.
    /// </summary>
    [JsonRequired]
    [JsonProperty("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Get or set name of item.
    /// </summary>
    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Get or set name of category.
    /// </summary>
    [JsonRequired]
    [JsonProperty("category")]
    public string Category { get; set; } = null!;

    /// <summary>
    /// Get or set count of items in storage.
    /// </summary>
    [JsonRequired]
    [JsonProperty("storage")]
    public int Storage { get; set; }

    /// <summary>
    /// Get or set retail price of item.
    /// </summary>
    [JsonRequired]
    [JsonProperty("price")]
    public float RetailPrice { get; set; }

    /// <summary>
    /// Get or set wholescale price of item.
    /// </summary>
    [JsonRequired]
    [JsonProperty("write_off")]
    public int WriteOff { get; set; }
}