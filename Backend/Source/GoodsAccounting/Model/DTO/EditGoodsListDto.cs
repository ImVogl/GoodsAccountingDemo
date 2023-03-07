using Newtonsoft.Json;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// DTO edition goods list storage.
/// </summary>
public class EditGoodsListDto
{
    /// <summary>
    /// Get or set user's identifier.
    /// </summary>
    [JsonRequired]
    [JsonProperty("user_id")]
    public int UserId { get; set; }

    /// <summary>
    /// Get or set item identifier or null for new item.
    /// </summary>
    [JsonProperty("id")]
    public Guid? Id { get; set; }

    /// <summary>
    /// Get or set value is indicating that editing type is creating new item.
    /// </summary>
    [JsonRequired]
    [JsonProperty("new")]
    public bool CreateNew { get; set; }

    /// <summary>
    /// Get or set value is indicating that editing type is removing item.
    /// </summary>
    [JsonRequired]
    [JsonProperty("remove")]
    public bool Remove { get; set; }

    /// <summary>
    /// Get or set value is indicating that editing type is restoring item.
    /// </summary>
    [JsonRequired]
    [JsonProperty("restore")]
    public bool Restore { get; set; }

    /// <summary>
    /// Get or set goods item name.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Get or set goods item category.
    /// </summary>
    [JsonProperty("category")]
    public string Category { get; set; } = null!;

    /// <summary>
    /// Get or set goods item store.
    /// </summary>
    [JsonProperty("store")]
    public int Store { get; set; }

    /// <summary>
    /// Get or set retail price.
    /// </summary>
    [JsonProperty("r_price")]
    public float RetailPrice { get; set; }

    /// <summary>
    /// Get or set wholescale price.
    /// </summary>
    [JsonProperty("w_price")]
    public float WholeScalePrice { get; set; }
}