using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GoodsAccounting.Model.DataBase;

/// <summary>
/// User database model.
/// </summary>
[Table("users")]
public class User
{
    /// <summary>
    /// Empty constructor for entity framework.
    /// </summary>
    public User()
    {
    }

    /// <summary>
    /// Get or set user's identifier.
    /// </summary>
    [Key]
    [Column("id", TypeName = "integer")]
    public int Id { get; set; }

    /// <summary>
    /// Get or set user login.
    /// </summary>
    [Column("login", TypeName = "text")]
    public string UserLogin { get; set; } = null!;

    /// <summary>
    /// Get or set user's name.
    /// </summary>
    [Column("name", TypeName = "text")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Get or set user's surname.
    /// </summary>
    [Column("surname", TypeName = "text")]
    public string Surname { get; set; } = null!;

    /// <summary>
    /// Get or set <see cref="UserRole"/>.
    /// </summary>
    [Column("role", TypeName = "text")]
    public string Role { get; set; } = null!;

    /// <summary>
    /// Get or set user's birth date.
    /// </summary>
    [Column("birth", TypeName = "date")]
    public DateOnly BirthDate { get; set;}

    /// <summary>
    /// Get or set hashed password.
    /// </summary>
    [MaxLength(32)]
    [Column("hash", TypeName = "bytea")]
    public byte[] Hash { get; set; } = null!;

    /// <summary>
    /// Get or set day when password will expire.
    /// </summary>
    [Column("expired", TypeName = "timestamp")]
    public DateTime PasswordExpired { get; set; }

    /// <summary>
    /// Get or set password salt.
    /// </summary>
    [Column("salt", TypeName = "text")]
    public string Salt { get; set; } = null!;
}