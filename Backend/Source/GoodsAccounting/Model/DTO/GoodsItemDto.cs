﻿using Newtonsoft.Json;

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
    [JsonProperty("r_price")]
    public float RetailPrice { get; set; }

    /// <summary>
    /// Get or set wholescale price of item.
    /// </summary>
    [JsonRequired]
    [JsonProperty("w_price")]
    public float WholeScalePrice { get; set; }

    /// <summary>
    /// Get or set goods in storage.
    /// </summary>
    [JsonRequired]
    [JsonProperty("storage")]
    public int Storage { get; set; }

    /// <summary>
    /// Get or set value is indicating that item is on the market.
    /// </summary>
    [JsonRequired]
    [JsonProperty("active")]
    public bool Actives { get; set; }
}