using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodsAccounting.Model.DataBase;

/// <summary>
/// Database model of goods item in the storage.
/// </summary>
[Table("goods_states")]
public class GoodsItemStorage
{
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public GoodsItemStorage()
    {
    }

    /// <summary>
    /// Get or set index.
    /// </summary>
    [Key]
    [Column("index", TypeName = "bigint")]
    public long Index { get; set; }

    /// <summary>
    /// Get or set item identifier.
    /// </summary>
    [Column("id", TypeName = "uuid")]
    public Guid Id { get; set; }
    
    /// <summary>
    /// Get or set number of written off items.
    /// </summary>
    [Column("write_off", TypeName = "integer")]
    public int WriteOff { get; set; }

    /// <summary>
    /// Get or set number of receipted items.
    /// </summary>
    [Column("receipt", TypeName = "integer")]
    public int Receipt { get; set; }

    /// <summary>
    /// Get or set number of goods in storage.
    /// </summary>
    [Column("storage", TypeName = "integer")]
    public int GoodsInStorage { get; set; }

    /// <summary>
    /// Get or set number of sold goods.
    /// </summary>
    [Column("sold", TypeName = "integer")]
    public int Sold { get; set; }

    /// <summary>
    /// Get or set retail price of item.
    /// </summary>
    [Column("retailPrice", TypeName = "real")]
    public float RetailPrice { get; set; }

    /// <summary>
    /// Get or set wholescale price of item.
    /// </summary>
    [Column("wholeScalePrice", TypeName = "real")]
    public float WholeScalePrice { get; set; }

    /// <summary>
    /// Get or set relationship for parent.
    /// </summary>
    public WorkShift ParentShift { get; set; }
}