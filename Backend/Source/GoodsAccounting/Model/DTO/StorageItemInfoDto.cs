using Newtonsoft.Json;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// DTO with target goods item history snapshot.
/// </summary>
public class StorageItemInfoDto
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
    /// Get or set number of written off items.
    /// </summary>
    [JsonRequired]
    [JsonProperty("write_off")]
    public int WriteOff { get; set; }

    /// <summary>
    /// Get or set number of receipted items.
    /// </summary>
    [JsonRequired]
    [JsonProperty("receipt")]
    public int Receipt { get; set; }

    /// <summary>
    /// Get or set number of goods in storage.
    /// </summary>
    [JsonRequired]
    [JsonProperty("storage")]
    public int GoodsInStorage { get; set; }

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
    [JsonProperty("r_price")]
    public float RetailPrice { get; set; }

    /// <summary>
    /// Get or set wholescale price of item.
    /// </summary>
    [JsonRequired]
    [JsonProperty("w_price")]
    public float WholeScalePrice { get; set; }

    /// <summary>
    /// Get or set income for this position.
    /// </summary>
    [JsonRequired]
    [JsonProperty("income")]
    public float Income => RetailPrice * Sold;

    /// <summary>
    /// Get or set spending that is connecting with wholescale purchase.
    /// </summary>
    [JsonRequired]
    [JsonProperty("wsp_spending")]
    public float WholesalePurchase => WholeScalePrice * Receipt;

    /// <summary>
    /// Get or set spending that connecting with writing-off event (wholescale price).
    /// </summary>
    [JsonRequired]
    [JsonProperty("wow_los")]
    public float WriteOffLossW => WholeScalePrice * WriteOff;

    /// <summary>
    /// Get or set spending that connecting with writing-off event (retail price).
    /// </summary>
    [JsonRequired]
    [JsonProperty("wor_los")]
    public float WriteOffLossR => RetailPrice * WriteOff;

}