using Newtonsoft.Json;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// DTO storage revision.
/// </summary>
public class GoodsRevisionDto
{
    /// <summary>
    /// Get or set user identifier.
    /// </summary>
    [JsonRequired]
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>
    /// Get or set list of <see cref="RevisionGoodsItemDto"/>.
    /// </summary>
    [JsonRequired]
    [JsonProperty("items")]
    public List<RevisionGoodsItemDto> Items { get; set; } = null!;
}