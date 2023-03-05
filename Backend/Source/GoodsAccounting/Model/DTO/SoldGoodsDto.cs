using Newtonsoft.Json;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// DTO with sold goods
/// </summary>
public class SoldGoodsDto
{
    /// <summary>
    /// Get or set dictionary where key: item guid; value: sold item count.
    /// </summary>
    [JsonRequired]
    [JsonProperty("sold")]
    public Dictionary<Guid, int> Sold { get; set; } = null!;
}