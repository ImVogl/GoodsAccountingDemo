using Newtonsoft.Json;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// DTO with target goods item reduced history snapshot.
/// </summary>
public class ReducedItemInfoDto
{
    /// <summary>
    /// Get or set goods item name.
    /// </summary>
    [JsonRequired]
    [JsonProperty("name")]
    public string ItemName { get; set; } = null!;

    /// <summary>
    /// Get or set goods item identifier.
    /// </summary>
    [JsonRequired]
    [JsonProperty("id")]
    public Guid ItemId { get; set; }

    /// <summary>
    /// Get or set number of sold goods.
    /// </summary>
    [JsonRequired]
    [JsonProperty("sold")]
    public int Sold { get; set; }

    /// <summary>
    /// Get or set retail price of item.
    /// </summary>
    [JsonRequired]
    [JsonProperty("price")]
    public float RetailPrice { get; set; }

}