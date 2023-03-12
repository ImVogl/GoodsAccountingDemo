using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodsAccounting.Model.DataBase;

/// <summary>
/// Database model of working shift.
/// </summary>
[Table("work_shift")]
public class WorkShift
{
#pragma warning disable CS8618
    /// <summary>
    /// Empty constructor. 
    /// </summary>
    public WorkShift()
    {
    }

#pragma warning restore CS8618

    /// <summary>
    /// Get or set entity index.
    /// </summary>
    [Key]
    [Required]
    [Column("index", TypeName = "serial")]
    public int Index { get; set; }

    /// <summary>
    /// Get or set shift day
    /// </summary>
    [Required]
    [Column("shift_open", TypeName = "timestamp")]
    public DateTime OpenTime { get; set; }

    /// <summary>
    /// Get or set shift day
    /// </summary>
    [Required]
    [Column("shift_close", TypeName = "timestamp")]
    public DateTime CloseTime { get; set; }

    /// <summary>
    /// Get or set user's displayed name.
    /// </summary>
    [Required]
    [Column("user_name", TypeName = "text")]
    public string UserDisplayName { get; set; } = null!;

    /// <summary>
    /// Get or set working shift manager/shop assistant.
    /// </summary>
    [Required]
    [Column("user_id", TypeName = "integer")]
    public int UserId { get; set; }

    /// <summary>
    /// Get or set cash in the cash box.
    /// </summary>
    [Required]
    [Column("cash", TypeName = "integer")]
    public int Cash { get; set; }

    /// <summary>
    /// Get or set value in indicating that shift is opened.
    /// </summary>
    [Required]
    [Column("opened", TypeName = "boolean")]
    public bool IsOpened { get; set; }

    /// <summary>
    /// Get or set comments for working shift.
    /// </summary>
    [Required]
    [Column("comments", TypeName = "text")]
    public string Comments { get; set; }

    /// <summary>
    /// Get or set collection of <see cref="GoodsItemStorage"/>.
    /// </summary>
    public List<GoodsItemStorage> GoodItemStates { get; set; }
}