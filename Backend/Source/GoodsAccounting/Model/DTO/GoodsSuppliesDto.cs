using Newtonsoft.Json;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// DTO storage supply information.
/// </summary>
public class GoodsSuppliesDto
{
    /// <summary>
    /// Get or set user identifier.
    /// </summary>
    [JsonRequired]
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>
    /// Get or set list of <see cref="GoodsItemSupplyDto"/>.
    /// </summary>
    [JsonRequired]
    [JsonProperty("items")]
    public List<GoodsItemSupplyDto> Items { get; set; } = null!;
}