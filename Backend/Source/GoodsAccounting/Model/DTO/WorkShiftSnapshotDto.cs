﻿using Newtonsoft.Json;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// DTO with snapshot of working shift history with aggregate info.
/// </summary>
public class WorkShiftSnapshotDto
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
    /// Get or set list of <see cref="StorageItemInfoDto"/>.
    /// </summary>
    [JsonRequired]
    [JsonProperty("storageItems")]
    public IList<StorageItemInfoDto> StorageItems { get; set; } = null!;
}