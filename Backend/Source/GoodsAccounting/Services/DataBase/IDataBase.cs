using GoodsAccounting.Model.DataBase;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace GoodsAccounting.Services.DataBase;

/// <summary>
/// Interface for data base proxy.
/// </summary>
public interface IDataBase
{
    /// <summary>
    /// Get collection of <see cref="User"/> entity.
    /// </summary>
    [NotNull]
    [ItemNotNull]
    DbSet<User> Users { get; }

    /// <summary>
    /// Change password for target user.
    /// </summary>
    /// <param name="id">User identifier.</param>
    /// <param name="salt">Password salt.</param>
    /// <param name="hash">Hashed password.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task ChangePasswordAsync(int id, string salt, byte[] hash);

    /// <summary>
    /// Adding new user.
    /// </summary>
    /// <param name="user"><see cref="User"/>.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddUser([NotNull] User user);
}