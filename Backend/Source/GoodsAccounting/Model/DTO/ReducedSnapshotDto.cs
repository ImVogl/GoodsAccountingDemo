using Newtonsoft.Json;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// DTO with reduced snapshot of working shift history with aggregate info.
/// </summary>
public class ReducedSnapshotDto
{
    /// <summary>
    /// Get or ser user's displayed name.
    /// </summary>
    [JsonRequired]
    [JsonProperty("name")]
    public string UserDisplayName { get; set; } = null!;

    /// <summary>
    /// Get or set cash amount in the cash register.
    /// </summary>
    [JsonRequired]
    [JsonProperty("cash")]
    public int Cash { get; set; }

    /// <summary>
    /// Get or set list of <see cref="ReducedItemInfoDto"/>.
    /// </summary>
    [JsonRequired]
    [JsonProperty("snapshots")]
    public IList<ReducedItemInfoDto> StorageItems { get; set; } = null!;
}