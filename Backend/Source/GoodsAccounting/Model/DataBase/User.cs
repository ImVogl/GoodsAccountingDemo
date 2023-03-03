namespace GoodsAccounting.Model.DataBase;

/// <summary>
/// User database model.
/// </summary>
public class User
{
    /// <summary>
    /// Get or set user's identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Get or set user login.
    /// </summary>
    public string UserLogin { get; set; } = null!;

    /// <summary>
    /// Get or set user's name.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Get or set user's surname.
    /// </summary>
    public string Surname { get; set; } = null!;

    /// <summary>
    /// Get or set <see cref="UserRole"/>.
    /// </summary>
    public string Role { get; set; } = null!;

    /// <summary>
    /// Get or set user's birth date.
    /// </summary>
    public DateOnly BirthDate { get; set;}

    /// <summary>
    /// Get or set hashed password.
    /// </summary>
    public byte[] Hash { get; set; } = null!;

    /// <summary>
    /// Get or set day when password will expire.
    /// </summary>
    public DateOnly PasswordExpired { get; set; }

    /// <summary>
    /// Get or set password salt.
    /// </summary>
    public string Salt { get; set; } = null!;
}