using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodsAccounting.Model.DataBase;

/// <summary>
/// Database model of goods item.
/// </summary>
[Table("goods")]
public class GoodsItem
{
#pragma warning disable CS8618
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public GoodsItem()
    {
    }

#pragma warning restore CS8618

    /// <summary>
    /// Get or set item identifier.
    /// </summary>
    [Key]
    [Column("id", TypeName = "uuid")]
    public Guid Id { get; set; }

    /// <summary>
    /// Get or set name of item.
    /// </summary>
    [Column("name", TypeName = "text")]
    public string Name { get; set; }

    /// <summary>
    /// Get or set category of item.
    /// </summary>
    [Column("category", TypeName = "text")]
    public string Category { get; set; }

    /// <summary>
    /// Get or set wholescale price of item.
    /// </summary>
    [Column("wholeScalePrice", TypeName = "real")]
    public float WholeScalePrice { get; set; }

    /// <summary>
    /// Get or set retail price of item.
    /// </summary>
    [Column("retailPrice", TypeName = "real")]
    public float RetailPrice { get; set; }

    /// <summary>
    /// Get or set count of items in storage.
    /// </summary>
    [Column("store", TypeName = "integer")]
    public int Storage { get; set; }

    /// <summary>
    /// Get or set value is indicating that item is on the market.
    /// </summary>
    [Column("active", TypeName = "boolean")]
    public bool Actives { get; set; }
}