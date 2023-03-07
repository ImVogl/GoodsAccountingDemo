using Newtonsoft.Json;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// DTO for goods item.
/// </summary>
public class GoodsItemDto
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
    /// Get or set category of item.
    /// </summary>
    [JsonRequired]
    [JsonProperty("category")]
    public string Category { get; set; } = null!;

    /// <summary>
    /// Get or set retail price of item.
    /// </summary>
    [JsonRequired]
    [JsonProperty("price")]
    public float Price { get; set; }
    
    /// <summary>
    /// Get or set value is indicating that item is on the market.
    /// </summary>
    [JsonRequired]
    [JsonProperty("active")]
    public bool Actives { get; set; }
}