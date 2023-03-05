﻿using Newtonsoft.Json;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// DTO for updatable goods item.
/// </summary>
public class UpdatingGoodsItemDto
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
    /// Get or set wholescale price of item.
    /// </summary>
    [JsonRequired]
    [JsonProperty("w_price")]
    public float WholeScalePrice { get; set; }

    /// <summary>
    /// Get or set retail price of item.
    /// </summary>
    [JsonRequired]
    [JsonProperty("r_price")]
    public float RetailPrice { get; set; }

    /// <summary>
    /// Get or set count of items in storage.
    /// </summary>
    [JsonRequired]
    [JsonProperty("storage")]
    public int CurrentItemsInStorageCount { get; set; }
}