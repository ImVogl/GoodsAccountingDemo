using Newtonsoft.Json;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// DTO update storage.
/// </summary>
public class UpdatingGoodsDto
{
    /// <summary>
    /// Get or set user identifier.
    /// </summary>
    [JsonRequired]
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>
    /// Get or set list with goods storage new state.
    /// </summary>
    [JsonRequired]
    [JsonProperty("items")]
    public List<UpdatingGoodsItemDto> Items { get; set; } = null!;

    /// <summary>
    /// Get or set list with goods to remove.
    /// </summary>
    [JsonRequired]
    [JsonProperty("remove")]
    public List<Guid> ItemsToRemove { get; set; } = null!;
}