using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodsAccounting.Model.DataBase;

/// <summary>
/// Database model of goods item.
/// </summary>
[Table("goods")]
public class GoodsItem
{
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public GoodsItem()
    {
    }

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
    /// Get or set value is indicating that item is on the market.
    /// </summary>
    [Column("active", TypeName = "boolean")]
    public bool Actives { get; set; }
}